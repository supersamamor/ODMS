namespace FBSC.ODMS.Core.Constants
{
    public static class ReportChartType
    {
        public const string Table = "Table";
        public const string HorizontalBar = "Horizontal Bar";
        public const string StackedHorizontalBar = "Stacked Horizontal Bar";
        public const string Bar = "Bar";
        public const string Pie = "Pie";
        public const string Doughnut = "Doughnut";
        public const string PolarArea = "Polar Area";
        public const string Line = "Line";
        public const string Radar = "Radar";
        public const string Bubble = "Bubble";
        public const string Scatter = "Scatter";
        public const string PDF = "PDF";
        public const string CustomHtml = "Custom Html";

        public const string ShortCodeFinePrint = @"
			<ul>
				<span>You can use the ff. short codes from the query.</span>
				<li>Current logged user`s id - {{CurrentUserId}}</li>
				<li>Date/time at the time of the report generation - {{CurrentDateTime}}</li>
			</ul>";

        // Shared guidance text: charts that plot one or more numeric series per
        // label (Horizontal Bar, Bar, Line, Radar) all expect the same query shape.
        private const string MultiSeriesQueryGuidance = @"<small>Your query should consist of one field with `label` field name and other fields should be a numeric value
							<br><quote>(eg. Select [Type] <b>Label</b>,
								Count(*) <b>[Transaction Count]</b>,
								Count(Distinct UserId) <b>[Number of Users]</b> From AuditLogs Group by [Type])</quote><br>";

        // Shared guidance text: charts that plot a single distribution across
        // categories (Pie, Doughnut, Polar Area) also share the same query shape.
        private const string DistributionQueryGuidance = @"<small>Your query should consist of one field with `label` field name and other fields should be a numeric value
							<br><quote>(eg. Select [Type] <b>Label</b>,
								Count(*) <b>[Transaction Count]</b>,
								Count(Distinct UserId) <b>[Number of Users]</b> From AuditLogs Group by [Type])</quote><br>";

        // Shared guidance text: charts that plot x/y (and optionally radius)
        // coordinate data (Bubble, Scatter) instead of category labels.
        private const string PointQueryGuidance = @"<small>Your query should consist of a `Label` field (used as the series/dataset name), and `X`, `Y` numeric fields for point position. Bubble charts additionally support an optional `R` numeric field for point radius.
							<br><quote>(eg. Select [Region] <b>Label</b>, [Latitude] <b>X</b>, [Longitude] <b>Y</b>, [PopulationDensity] <b>R</b> From Regions)</quote><br>";

        public static readonly Dictionary<string, string> ChartToolTip = new()
        {
            { Table, "<small>" + ShortCodeFinePrint +"</small>" },
            { HorizontalBar, MultiSeriesQueryGuidance + ShortCodeFinePrint + "</small>" },
            { StackedHorizontalBar, MultiSeriesQueryGuidance + ShortCodeFinePrint + "</small>" },
            { Bar, MultiSeriesQueryGuidance + ShortCodeFinePrint + "</small>" },
            { Pie, DistributionQueryGuidance + ShortCodeFinePrint + "</small>" },
            { Doughnut, DistributionQueryGuidance + ShortCodeFinePrint + "</small>" },
            { PolarArea, DistributionQueryGuidance + ShortCodeFinePrint + "</small>" },
            { Line, MultiSeriesQueryGuidance + ShortCodeFinePrint + "</small>" },
            { Radar, MultiSeriesQueryGuidance + ShortCodeFinePrint + "</small>" },
            { Bubble, PointQueryGuidance + ShortCodeFinePrint + "</small>" },
            { Scatter, PointQueryGuidance + ShortCodeFinePrint + "</small>" },
            { PDF, "<small>Provide html template for PDF Report.</small>" },
            { CustomHtml, "<small>Provide html template for the Report.</small>" },
        };

        public const string HorizontalBarQueryFormat = @"The query should have the following format for " + HorizontalBar + " Chart: Select {field where the label will come from} [Label], Count(*) {Label of the count} From {TableName}";
        public const string StackedHorizontalBarQueryFormat = @"The query should have the following format for " + StackedHorizontalBar + " Chart: Select {field where the label will come from} [Label], {numeric field per stack segment, one column each} From {TableName}";
        public const string BarQueryFormat = @"The query should have the following format for " + Bar + " Chart: Select {field where the label will come from} [Label], Count(*) {Label of the count} From {TableName}";
        public const string PieQueryFormat = @"The query should have the following format for " + Pie + " Chart: Select {field where the label will come from} [Label], Count(*) {Label of the count} From {TableName}";
        public const string DoughnutQueryFormat = @"The query should have the following format for " + Doughnut + " Chart: Select {field where the label will come from} [Label], Count(*) {Label of the count} From {TableName}";
        public const string PolarAreaQueryFormat = @"The query should have the following format for " + PolarArea + " Chart: Select {field where the label will come from} [Label], Count(*) {Label of the count} From {TableName}";
        public const string LineQueryFormat = @"The query should have the following format for " + Line + " Chart: Select {field where the label will come from} [Label], Count(*) {Label of the count} From {TableName}";
        public const string RadarQueryFormat = @"The query should have the following format for " + Radar + " Chart: Select {field where the label will come from} [Label], Count(*) {Label of the count} From {TableName}";
        public const string BubbleQueryFormat = @"The query should have the following format for " + Bubble + " Chart: Select {series name field} [Label], {x field} [X], {y field} [Y], {optional radius field} [R] From {TableName}";
        public const string ScatterQueryFormat = @"The query should have the following format for " + Scatter + " Chart: Select {series name field} [Label], {x field} [X], {y field} [Y] From {TableName}";

        // Lookup so callers (e.g. the AI query-generation prompt) don't need
        // their own switch statement that has to be kept in sync with the
        // chart type list above.
        public static readonly Dictionary<string, string> ChartQueryFormat = new()
        {
            { HorizontalBar, HorizontalBarQueryFormat },
            { StackedHorizontalBar, StackedHorizontalBarQueryFormat },
            { Bar, BarQueryFormat },
            { Pie, PieQueryFormat },
            { Doughnut, DoughnutQueryFormat },
            { PolarArea, PolarAreaQueryFormat },
            { Line, LineQueryFormat },
            { Radar, RadarQueryFormat },
            { Bubble, BubbleQueryFormat },
            { Scatter, ScatterQueryFormat },
        };
    }
}