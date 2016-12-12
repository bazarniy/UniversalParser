namespace Extraction.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base.Helpers;
    using Base.Utilities;
    using HtmlAgilityPack;

    public class NormalizedTable
    {
        private readonly HtmlNode[,] _table;
        private readonly int _rows;
        private readonly int _cols;
        private bool _transposed;

        public NormalizedTable(HtmlNode htmltable)
        {
            htmltable.ThrowIfNull(nameof(htmltable));
            if(htmltable.Name!="table") throw new ArgumentException("Not table node", nameof(htmltable));
            
            var result = htmltable.SelectNodesSafe("tr|*/tr")
                .Select(row =>
                    row.SelectNodesSafe("th|td")
                        .Select(cell => new Cell(cell))
                        .ToList()
                        )
                .ToList();

            if (!result.Any())
            {
                _rows = 0;
                _cols = 0;
                return;
            }

            for (var i = 0; i < result.Count; i++)
                for (var j = 0; j < result[i].Count; j++)
                {
                    var colspan = result[i][j].Colspan;
                    if (colspan > 1)
                    {
                        result[i][j].Colspan = 1;

                        for (var k = 1; k < colspan; k++)
                            result[i].Insert(j + k, new Cell(result[i][j].Node) {Colspan = result[i][j].Colspan});
                    }

                    var rowspan = result[i][j].Rowspan;
                    if (rowspan <= 1) continue;

                    result[i][j].Rowspan = 1;

                    for (var k = 1; k < rowspan; k++)
                        result[i + k].Insert(j, result[i][j]);
                }

            var maxCols = result.Select(x => x.Count).Max();
            foreach (var source in result.Where(x => x.Count < maxCols))
                while (source.Count < maxCols) source.Add(null);

            _rows = result.Count;
            _cols = maxCols;

            _table = new HtmlNode[_rows, _cols];
            for (var i = 0; i < _rows; i++)
                for (var j = 0; j < _cols; j++)
                    _table[i, j] = result[i][j]?.Node;
        }

        public HtmlNode this[int row, int col] => !_transposed ? _table[row, col] : _table[col, row];

        public void Transpose()
        {
            _transposed = !_transposed;
        }

        private class Cell
        {
            public readonly HtmlNode Node;
            public int Colspan;
            public int Rowspan;

            public Cell(HtmlNode node)
            {
                Node = node;
                Colspan = node.GetColspan();
                Rowspan = node.GetRowspan();
            }
        }
    }
}