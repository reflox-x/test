using Microsoft.Maui.Graphics;

namespace pawledger.Services;

public class HomeChartDrawable : IDrawable
{
    public List<float> IncomePoints { get; set; } = new();
    public List<float> ExpensePoints { get; set; } = new();

    public int DaysInMonth { get; set; } = 30;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float width = dirtyRect.Width;
        float height = dirtyRect.Height;

        float leftPadding = 45;
        float rightPadding = 10;
        float topPadding = 10;
        float bottomPadding = 28;   // 留空间给刻度

        float chartWidth = width - leftPadding - rightPadding;
        float chartHeight = height - topPadding - bottomPadding;

        if (chartWidth <= 0 || chartHeight <= 0)
            return;

        float maxValue = 1;

        if (IncomePoints.Count > 0)
            maxValue = Math.Max(maxValue, IncomePoints.Max());

        if (ExpensePoints.Count > 0)
            maxValue = Math.Max(maxValue, ExpensePoints.Max());

        canvas.StrokeColor = Colors.LightGray;
        canvas.StrokeSize = 1;
        canvas.FontColor = Colors.Gray;
        canvas.FontSize = 11;

        // 左侧金额刻度 横线
        for (int i = 0; i <= 3; i++)
        {
            float ratio = i / 3f;
            float y = topPadding + chartHeight - (ratio * chartHeight);

            canvas.DrawLine(leftPadding, y, leftPadding + chartWidth, y);

            float value = maxValue * ratio;
            string label = ((int)value).ToString();

            canvas.DrawString(
                label,
                0,
                y - 8,
                leftPadding - 6,
                16,
                HorizontalAlignment.Right,
                VerticalAlignment.Center);
        }

        // 底部日期刻度
        DrawBottomAxis(canvas, leftPadding, topPadding, chartWidth, chartHeight);

        DrawLineSeries(canvas, IncomePoints, leftPadding, topPadding, chartWidth, chartHeight, maxValue, Color.FromArgb("#F0C84B"));
        DrawLineSeries(canvas, ExpensePoints, leftPadding, topPadding, chartWidth, chartHeight, maxValue, Color.FromArgb("#F48C7F"));
    }

    private void DrawBottomAxis(ICanvas canvas, float leftPadding, float topPadding, float chartWidth, float chartHeight)
    {
        if (DaysInMonth < 2)
            return;

        float baseY = topPadding + chartHeight + 4;

        // 5个刻度
        List<int> tickDays = new() { 1 };

        if (DaysInMonth >= 8) tickDays.Add(8);
        if (DaysInMonth >= 15) tickDays.Add(15);
        if (DaysInMonth >= 22) tickDays.Add(22);
        if (DaysInMonth > 22) tickDays.Add(DaysInMonth);

        tickDays = tickDays.Distinct().ToList();

        foreach (int day in tickDays)
        {
            float x;

            if (DaysInMonth == 1)
                x = leftPadding;
            else
                x = leftPadding + ((day - 1f) / (DaysInMonth - 1f)) * chartWidth;

            // 小刻度线
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 1;
            canvas.DrawLine(x, topPadding + chartHeight, x, topPadding + chartHeight + 4);

            // 日期文字
            canvas.DrawString(
                day.ToString(),
                x - 12,
                baseY,
                24,
                16,
                HorizontalAlignment.Center,
                VerticalAlignment.Top);
        }
    }

    private void DrawLineSeries(
        ICanvas canvas,
        List<float> points,
        float leftPadding,
        float topPadding,
        float chartWidth,
        float chartHeight,
        float maxValue,
        Color color)
    {
        if (points == null || points.Count < 2)
            return;

        canvas.StrokeColor = color;
        canvas.StrokeSize = 3;

        float stepX = chartWidth / (points.Count - 1);

        for (int i = 0; i < points.Count - 1; i++)
        {
            float x1 = leftPadding + i * stepX;
            float y1 = topPadding + chartHeight - (points[i] / maxValue * chartHeight);

            float x2 = leftPadding + (i + 1) * stepX;
            float y2 = topPadding + chartHeight - (points[i + 1] / maxValue * chartHeight);

            canvas.DrawLine(x1, y1, x2, y2);

            canvas.FillColor = color;
            canvas.FillCircle(x1, y1, 4);
        }

        float lastX = leftPadding + (points.Count - 1) * stepX;
        float lastY = topPadding + chartHeight - (points.Last() / maxValue * chartHeight);

        canvas.FillColor = color;
        canvas.FillCircle(lastX, lastY, 4);
    }
}