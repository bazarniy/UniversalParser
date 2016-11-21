namespace Extraction.Common
{
    using System.Linq;
    using Base.Helpers;
    using HtmlAgilityPack;

    public class NormalizedTable
    {
        private readonly HtmlNode[,] _table;
        private readonly int _rows;
        private readonly int _cols;
        private bool _transposed;

        public NormalizedTable(HtmlNode htmltable)
        {
            //TEST: <table><thead><tr><td colspan=\"2\" rowspan=\"2\">1</td><td>2</td></tr><tr><td>3</td><td>4</td></tr></thead><tr><td>3</td><td>4</td></tr><tbody><tr><td>3</td><td>4</td></tr></tbody></table>

            var result = htmltable.SelectNodes("tr|*/tr")
                .Select(row =>
                    row.SelectNodes("th|td")
                        .Select(cell => new Cell(cell))
                        .ToList())
                .ToList();

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