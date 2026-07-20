using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Web.Areas.Identity.Data;

public static class DefaultDashboard
{
    public static async Task Seed(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>(), serviceProvider.GetRequiredService<IAuthenticatedUser>());
        var entity = await context.Report.FirstOrDefaultAsync(e => e.ReportName == "Activity Logs - Horizontal Bar Graph");
        if (entity == null)
        {
            context.Report.Add(new ReportState()
            {
                Id = Guid.NewGuid().ToString(),
                ReportName = "Activity Logs - Custom Html",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.CustomHtml,
                IsDistinct = false,
                QueryString = @"SELECT 
                    SUM(CASE WHEN [Type] = 'Create' THEN 1 ELSE 0 END) AS [CreateCount],
                    SUM(CASE WHEN [Type] = 'Update' THEN 1 ELSE 0 END) AS [UpdateCount],
                    SUM(CASE WHEN [Type] = 'User logged in' THEN 1 ELSE 0 END) AS [LoginCount],
                    SUM(CASE WHEN [Type] = 'User logged out' THEN 1 ELSE 0 END) AS [LogoutCount],
                    SUM(CASE WHEN [Type] = 'Invalid login attempt' THEN 1 ELSE 0 END) AS [InvalidLoginCount]
                FROM [dbo].[AuditLogs]
                WHERE (@DateFrom = '' OR [DateTime] >= @DateFrom)
                  AND (@DateTo = '' OR [DateTime] <= @DateTo)
                OPTION (RECOMPILE);",
                DisplayOnDashboard = true,
                DisplayOnReportModule = true,
                HtmlTemplate = """
                <div class="stats-cards-container" style="display: flex; flex-wrap: nowrap; overflow-x: auto; gap: 15px; padding: 10px;">
                    <!--{#foreach Table}-->
        
                    <!-- Creates Card -->
                    <div style="flex: 1 1 0; min-width: 140px; background-color: var(--bg-light); border-radius: var(--border-radius-base); border-left: 4px solid var(--custom-primary); box-shadow: 0 .125rem .25rem rgba(0,0,0,.075); padding: 15px;">
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <div>
                                <div style="font-size: var(--font-size-dense); color: var(--custom-primary); font-weight: 700; text-transform: uppercase; margin-bottom: 5px;">Creates</div>
                                <div style="font-size: 1.5rem; font-weight: 700; color: #5a5c69;">{CreateCount}</div>
                            </div>
                            <div><i class="fas fa-plus-circle fa-2x" style="color: #dddfeb;"></i></div>
                        </div>
                    </div>

                    <!-- Updates Card -->
                    <div style="flex: 1 1 0; min-width: 140px; background-color: var(--bg-light); border-radius: var(--border-radius-base); border-left: 4px solid var(--color-warning); box-shadow: 0 .125rem .25rem rgba(0,0,0,.075); padding: 15px;">
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <div>
                                <div style="font-size: var(--font-size-dense); color: var(--color-warning); font-weight: 700; text-transform: uppercase; margin-bottom: 5px;">Updates</div>
                                <div style="font-size: 1.5rem; font-weight: 700; color: #5a5c69;">{UpdateCount}</div>
                            </div>
                            <div><i class="fas fa-edit fa-2x" style="color: #dddfeb;"></i></div>
                        </div>
                    </div>

                    <!-- Logged In Card -->
                    <div style="flex: 1 1 0; min-width: 140px; background-color: var(--bg-light); border-radius: var(--border-radius-base); border-left: 4px solid var(--color-success); box-shadow: 0 .125rem .25rem rgba(0,0,0,.075); padding: 15px;">
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <div>
                                <div style="font-size: var(--font-size-dense); color: var(--color-success); font-weight: 700; text-transform: uppercase; margin-bottom: 5px;">Logins</div>
                                <div style="font-size: 1.5rem; font-weight: 700; color: #5a5c69;">{LoginCount}</div>
                            </div>
                            <div><i class="fas fa-sign-in-alt fa-2x" style="color: #dddfeb;"></i></div>
                        </div>
                    </div>

                    <!-- Logged Out Card -->
                    <div style="flex: 1 1 0; min-width: 140px; background-color: var(--bg-light); border-radius: var(--border-radius-base); border-left: 4px solid var(--color-violet); box-shadow: 0 .125rem .25rem rgba(0,0,0,.075); padding: 15px;">
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <div>
                                <div style="font-size: var(--font-size-dense); color: var(--color-violet); font-weight: 700; text-transform: uppercase; margin-bottom: 5px;">Logouts</div>
                                <div style="font-size: 1.5rem; font-weight: 700; color: #5a5c69;">{LogoutCount}</div>
                            </div>
                            <div><i class="fas fa-sign-out-alt fa-2x" style="color: #dddfeb;"></i></div>
                        </div>
                    </div>

                    <!-- Invalid Login Card -->
                    <div style="flex: 1 1 0; min-width: 140px; background-color: var(--bg-light); border-radius: var(--border-radius-base); border-left: 4px solid var(--color-danger); box-shadow: 0 .125rem .25rem rgba(0,0,0,.075); padding: 15px;">
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <div>
                                <div style="font-size: var(--font-size-dense); color: var(--color-danger); font-weight: 700; text-transform: uppercase; margin-bottom: 5px;">Invalid Logins</div>
                                <div style="font-size: 1.5rem; font-weight: 700; color: #5a5c69;">{InvalidLoginCount}</div>
                            </div>
                            <div><i class="fas fa-exclamation-triangle fa-2x" style="color: #dddfeb;"></i></div>
                        </div>
                    </div>

                    <!--{/foreach}-->
                </div>
                """,
                Sequence = 0,
                SpanWidth = 100,
                ReportRoleAssignmentList = [
                   new ReportRoleAssignmentState()
                    {
                        Id =   Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
               ],
                ReportQueryFilterList = [
                   new ReportQueryFilterState()
                    {
                        Id =   Guid.NewGuid().ToString(),
                        FieldName = "DateFrom",
                        FieldDescription = "Date From",
                        DataType = "Date",
                        Sequence = 0,
                    },
                    new ReportQueryFilterState()
                    {
                        Id =   Guid.NewGuid().ToString(),
                        FieldName = "DateTo",
                        FieldDescription = "Date To",
                        DataType = "Date",
                        Sequence = 1,
                    }
               ]
            });
            await context.SaveChangesAsync();//
            context.Report.Add(new ReportState()
            {
                Id = Guid.NewGuid().ToString(),
                ReportName = "Activity Logs - Horizontal Bar Graph",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.HorizontalBar,
                IsDistinct = false,
                QueryString = @"SELECT
                                  [Type] [Label]
                                 ,count(*) [Data]
                              FROM [dbo].[AuditLogs]
                              group by [Type]",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                Sequence = 1,
                SpanWidth = 50,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        Id =   Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
				]
            });
            await context.SaveChangesAsync();
            context.Report.Add(new ReportState()
            {
                Id = Guid.NewGuid().ToString(),
                ReportName = "Activity Logs - Pie Chart",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.Pie,
                IsDistinct = false,
                QueryString = @"SELECT
                                  [Type] [Label]
                                 ,count(*) [Data]
                              FROM [dbo].[AuditLogs]
                              group by [Type]",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                Sequence = 2,
                SpanWidth = 50,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        Id =   Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
            context.Report.Add(new ReportState()
            {
                Id = Guid.NewGuid().ToString(),
                ReportName = "Activity Logs - Table",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.Table,
                IsDistinct = false,
                QueryString = @"SELECT
                              [Type] [Activity]
                             ,count(*) [Count]
                          FROM [dbo].[AuditLogs]
                          group by [Type]",
                DisplayOnDashboard = true,
                DisplayOnReportModule = true,
                Sequence = 3,
                SpanWidth = 50,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        Id =   Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
			context.Report.Add(new ReportState()
			{
				Id = Guid.NewGuid().ToString(),
				ReportName = "Activity Logs - Pdf",
				QueryType = Core.Constants.QueryType.TSql,
				ReportOrChartType = Core.Constants.ReportChartType.PDF,
				IsDistinct = false,
				QueryString = @"SELECT
                        [Type] AS [Activity],
                        COUNT(*) AS [Count]
                    FROM [dbo].[AuditLogs]
                    WHERE (@DateFrom = '' OR [DateTime] >= @DateFrom)
                      AND (@DateTo = '' OR [DateTime] <= @DateTo)
                    GROUP BY [Type]
                    OPTION (RECOMPILE);",
				DisplayOnDashboard = false,
				DisplayOnReportModule = true,
				HtmlTemplate = "<div class=\"report\" style=\"font-family: Arial, sans-serif; max-width: 850px; margin: 0 auto; padding: 20px; color: #333;\">\r\n  \r\n" +
					"  <!-- HEADER -->\r\n  " +
					"<div class=\"report-header\" style=\"text-align: center; margin-bottom: 25px;\">\r\n   " +
					" <h1 style=\"font-size: 28px; margin: 0; letter-spacing: 0.5px; font-weight: 600; color: #222;\">\r\n   " +
					"   {ReportName}\r\n    </h1>\r\n    <hr style=\"margin-top: 18px; border: 0; border-top: 2px solid #444;\">\r\n" +
					"  </div>\r\n\r\n  <!-- TABLE -->\r\n  <table class=\"report-table\" role=\"table\" style=\"width: 100%; border-collapse: collapse; margin-bottom: 25px; font-size: 14px;\">\r\n    \r\n  " +
					"  <thead>\r\n      <tr style=\"background: #f2f2f2; border-bottom: 2px solid #999;\">\r\n      " +
					"  <th style=\"padding: 10px 8px; text-align: left; font-weight: 600;\">Activity</th>\r\n       " +
					" <th style=\"padding: 10px 8px; text-align: right; font-weight: 600;\">Count</th>\r\n    " +
					"  </tr>\r\n    </thead>\r\n\r\n    <tbody>\r\n      <!--{#foreach Table}-->\r\n   " +
					"   <tr style=\"border-bottom: 1px solid #ddd;\">\r\n      " +
					"  <td style=\"padding: 8px 6px;\">{Activity}</td>\r\n      " +
					"  <td style=\"padding: 8px 6px; text-align: right;\">{Count}</td>\r\n    " +
					"  </tr>\r\n      <!--{/foreach}-->\r\n    </tbody>\r\n  </table>\r\n\r\n " +
					" <!-- FOOTER -->\r\n  <div class=\"footer\" style=\"font-size: 12px; color: #555; display: flex; justify-content: space-between; border-top: 1px solid #ccc; padding-top: 10px;\">\r\n  " +
					"  <div class=\"small\">Distinct activities: {Table.Rows.Count}</div>\r\n    " +
					"<div class=\"small\">Generated: {ReportGenerated}</div>\r\n  </div>\r\n\r\n</div>\r\n",
				PaperSize = nameof(Rotativa.AspNetCore.Options.Size.A4),
				Orientation = nameof(Rotativa.AspNetCore.Options.Orientation.Portrait),
				MarginTop = 10,
				MarginBottom = 10,
				MarginLeft = 10,
				MarginRight = 10,
				Sequence = 4,
                SpanWidth = 100,
				ReportRoleAssignmentList = [
				   new ReportRoleAssignmentState()
					{
						Id =   Guid.NewGuid().ToString(),
						RoleName = Core.Constants.Roles.Admin
					}
			   ],
				ReportQueryFilterList = [
				   new ReportQueryFilterState()
					{
						Id =   Guid.NewGuid().ToString(),
						FieldName = "DateFrom",
						FieldDescription = "Date From",
						DataType = "Date",
						Sequence = 0,
					},
					new ReportQueryFilterState()
					{
						Id =   Guid.NewGuid().ToString(),
						FieldName = "DateTo",
						FieldDescription = "Date To",
						DataType = "Date",
						Sequence = 1,
					}
			   ]
			});
			await context.SaveChangesAsync();
        }

        // Seeded separately (own existence gate) so these land on databases
        // that already carry the Activity Logs seeds above.
        var healthByBu = await context.Report.FirstOrDefaultAsync(e => e.ReportName == "Health by BU");
        if (healthByBu == null)
        {
            // Widget 1: stacked RAG bars with per-row totals. Static dummy data;
            // swap the VALUES block for a real GROUP BY over Project/BusinessUnit
            // once live data exists.
            context.Report.Add(new ReportState()
            {
                Id = Guid.NewGuid().ToString(),
                ReportName = "Health by BU",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.StackedHorizontalBar,
                IsDistinct = false,
                QueryString = @"SELECT [Label], [Green], [Amber], [Red]
                    FROM (VALUES
                        ('Filinvest Land, Inc.', 17, 7, 4),
                        ('Filinvest Alabang, Inc.', 13, 6, 3),
                        ('FDC Utilities, Inc.', 11, 4, 3),
                        ('Countrywide Water Services, Inc.', 9, 4, 2),
                        ('Chroma Hospitality, Inc.', 7, 3, 2),
                        ('SharePro, Inc.', 6, 2, 1)
                    ) v([Label], [Green], [Amber], [Red]);",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                Sequence = 5,
                SpanWidth = 50,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });

            // Widget 2: budget-vs-target progress bars. Not a chart - Custom Html
            // with every presentational value (percent widths, colors, badge class,
            // delta label) precomputed in the query so the template is dumb
            // substitution. Percentages share one scale so bar lengths compare
            // across BUs; the blue tick marks each BU's budget target.
            context.Report.Add(new ReportState()
            {
                Id = Guid.NewGuid().ToString(),
                ReportName = "Budget Performance by BU",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.CustomHtml,
                IsDistinct = false,
                QueryString = @"SELECT [BusinessUnit], [ActualM], [DeltaLabel], [BadgeClass], [FillPct], [TargetPct], [BarColor]
                    FROM (VALUES
                        ('Filinvest Land', 312, '-28M', 'badge-soft-success', 92, 100, 'var(--color-success)'),
                        ('Filinvest Alabang', 295, '+15M', 'badge-soft-danger', 87, 82, 'var(--color-danger)'),
                        ('FDC Utilities', 198, '-22M', 'badge-soft-success', 58, 65, 'var(--color-success)'),
                        ('Countrywide Water', 172, '-8M', 'badge-soft-success', 51, 53, 'var(--color-success)'),
                        ('Chroma Hospitality', 115, '-5M', 'badge-soft-success', 34, 35, 'var(--color-success)'),
                        ('SharePro', 88, '-2M', 'badge-soft-success', 26, 27, 'var(--color-success)')
                    ) v([BusinessUnit], [ActualM], [DeltaLabel], [BadgeClass], [FillPct], [TargetPct], [BarColor]);",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                HtmlTemplate = """
                <div style="padding: 6px 4px;">
                    <!--{#foreach Table}-->
                    <div style="margin-bottom: 16px;">
                        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px;">
                            <span style="font-size: var(--font-size-dense); color: var(--text-muted);">{BusinessUnit}</span>
                            <span style="display: inline-flex; align-items: center; gap: 6px;">
                                <strong style="font-size: var(--font-size-dense); color: var(--text-color);">&#8369;{ActualM}M</strong>
                                <span class="badge-soft {BadgeClass}">{DeltaLabel}</span>
                            </span>
                        </div>
                        <div style="position: relative; height: 12px; background-color: #e8ebf3; border-radius: 999px;">
                            <div style="position: absolute; left: 0; top: 0; height: 12px; width: {FillPct}%; background-color: {BarColor}; border-radius: 999px;"></div>
                            <div style="position: absolute; left: {TargetPct}%; top: -2px; height: 16px; width: 3px; background-color: #3b5bdb; border-radius: 2px;"></div>
                        </div>
                    </div>
                    <!--{/foreach}-->
                    <div style="display: flex; gap: 18px; margin-top: 4px; font-size: var(--font-size-dense); color: var(--text-muted);">
                        <span><span style="display: inline-block; width: 10px; height: 10px; border-radius: 50%; background-color: var(--color-success); margin-right: 5px;"></span>Within budget</span>
                        <span><span style="display: inline-block; width: 10px; height: 10px; border-radius: 50%; background-color: var(--color-danger); margin-right: 5px;"></span>Over budget</span>
                        <span><span style="display: inline-block; width: 3px; height: 12px; background-color: #3b5bdb; margin-right: 5px;"></span>Budget target</span>
                    </div>
                </div>
                """,
                Sequence = 6,
                SpanWidth = 50,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
        }
    }
}
