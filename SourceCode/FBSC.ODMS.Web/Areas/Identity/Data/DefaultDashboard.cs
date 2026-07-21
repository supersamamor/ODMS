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
                DisplayOnDashboard = false,
                DisplayOnReportModule = false,
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
                DisplayOnDashboard = false,
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
                DisplayOnDashboard = false,
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
                DisplayOnDashboard = false,
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

        // KPI stat-card row (Total / Green / Amber / Red projects). Custom Html
        // with a static single-row query; swap for real counts over Project
        // grouped by HealthStatus once live data exists.
        var portfolioSummary = await context.Report.FirstOrDefaultAsync(e => e.ReportName == "Project Portfolio Summary");
        if (portfolioSummary == null)
        {
            var portfolioSummaryReportId = Guid.NewGuid().ToString();
            context.Report.Add(new ReportState()
            {
                Id = portfolioSummaryReportId,
                ReportName = "Project Portfolio Summary",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.CustomHtml,
                IsDistinct = false,
                QueryString = @"SELECT 104 AS [TotalProjects], 65 AS [GreenProjects], 25 AS [AmberProjects], 14 AS [RedProjects];",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                HtmlTemplate = """
                <div style="display: flex; flex-wrap: wrap; gap: 16px;">
                    <!--{#foreach Table}-->

                    <!-- Total Projects -->
                    <div style="flex: 1 1 200px; min-width: 180px; background-color: #ffffff; border: 1px solid #e9ecf2; border-radius: 12px; padding: 18px 20px;">
                        <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 10px;">
                            <span style="font-size: var(--font-size-base); font-weight: 600; color: #1e293b;">Total Projects</span>
                            <span style="display: inline-flex; align-items: center; justify-content: center; width: 34px; height: 34px; border-radius: 8px; background-color: #e8edfc; color: #2563eb;"><i class="fas fa-folder"></i></span>
                        </div>
                        <div style="font-size: 1.6rem; font-weight: 700; color: #0f172a; margin-top: 14px;">{TotalProjects}</div>
                        <div style="font-size: var(--font-size-dense); font-family: var(--font-secondary); color: #64748b; margin-top: 2px;">Across all BUs</div>
                    </div>

                    <!-- Green Projects -->
                    <div style="flex: 1 1 200px; min-width: 180px; background-color: #ffffff; border: 1px solid #e9ecf2; border-radius: 12px; padding: 18px 20px;">
                        <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 10px;">
                            <span style="font-size: var(--font-size-base); font-weight: 600; color: #1e293b;">Green Projects</span>
                            <span style="display: inline-flex; align-items: center; justify-content: center; width: 34px; height: 34px; border-radius: 8px; background-color: #e2f6ee; color: #10b981;"><i class="fas fa-check-circle"></i></span>
                        </div>
                        <div style="font-size: 1.6rem; font-weight: 700; color: #0f172a; margin-top: 14px;">{GreenProjects}</div>
                        <div style="font-size: var(--font-size-dense); font-family: var(--font-secondary); color: #64748b; margin-top: 2px;">On track</div>
                    </div>

                    <!-- Amber Projects -->
                    <div style="flex: 1 1 200px; min-width: 180px; background-color: #ffffff; border: 1px solid #e9ecf2; border-radius: 12px; padding: 18px 20px;">
                        <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 10px;">
                            <span style="font-size: var(--font-size-base); font-weight: 600; color: #1e293b;">Amber Projects</span>
                            <span style="display: inline-flex; align-items: center; justify-content: center; width: 34px; height: 34px; border-radius: 8px; background-color: #fdf3d9; color: #f59e0b;"><i class="fas fa-clock"></i></span>
                        </div>
                        <div style="font-size: 1.6rem; font-weight: 700; color: #0f172a; margin-top: 14px;">{AmberProjects}</div>
                        <div style="font-size: var(--font-size-dense); font-family: var(--font-secondary); color: #64748b; margin-top: 2px;">Require monitoring</div>
                    </div>

                    <!-- Red Projects -->
                    <div style="flex: 1 1 200px; min-width: 180px; background-color: #ffffff; border: 1px solid #e9ecf2; border-radius: 12px; padding: 18px 20px;">
                        <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 10px;">
                            <span style="font-size: var(--font-size-base); font-weight: 600; color: #1e293b;">Red Projects</span>
                            <span style="display: inline-flex; align-items: center; justify-content: center; width: 34px; height: 34px; border-radius: 8px; background-color: #fbeae8; color: #ef4444;"><i class="fas fa-exclamation-triangle"></i></span>
                        </div>
                        <div style="font-size: 1.6rem; font-weight: 700; color: #0f172a; margin-top: 14px;">{RedProjects}</div>
                        <div style="font-size: var(--font-size-dense); font-family: var(--font-secondary); color: #64748b; margin-top: 2px;">Needs immediate action</div>
                    </div>

                    <!--{/foreach}-->
                </div>
                """,
                // -1 sorts ahead of every existing widget (lowest Sequence today
                // is 0) so the KPI row leads the dashboard without renumbering.
                Sequence = 0,
                SpanWidth = 100,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        ReportId = portfolioSummaryReportId,
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
        }


        // Doughnut of project counts per BU with a right-side value legend.
        // Static dummy data; column order matters - the static chart palette
        // assigns blue/sky/emerald/orange/violet/slate in row order, matching
        // the reference design's BU colors. Swap the VALUES block for a real
        // GROUP BY over Project/BusinessUnit once live data exists.
        var projectsByBu = await context.Report.FirstOrDefaultAsync(e => e.ReportName == "Projects by BU");
        if (projectsByBu == null)
        {
            var projectsByBuReportId = Guid.NewGuid().ToString();
            context.Report.Add(new ReportState()
            {
                Id = projectsByBuReportId,
                ReportName = "Projects by BU",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.Doughnut,
                IsDistinct = false,
                QueryString = @"SELECT [Label], [Projects]
                    FROM (VALUES
                        ('Filinvest Land, Inc.', 28),
                        ('Filinvest Alabang, Inc.', 22),
                        ('FDC Utilities, Inc.', 18),
                        ('Countrywide Water Services, Inc.', 15),
                        ('Chroma Hospitality, Inc.', 12),
                        ('SharePro, Inc.', 33)
                    ) v([Label], [Projects]);",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                Sequence = 1,
                SpanWidth = 33,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        ReportId = projectsByBuReportId,
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
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
                Sequence = 2,
                SpanWidth = 33,
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

        var budgetByBu = await context.Report
            .Include(r => r.ReportRoleAssignmentList)
            .Include(r => r.ReportQueryFilterList)
            .FirstOrDefaultAsync(e => e.ReportName == "Budget Performance by BU");

        // One-time migration: the first version of this seed colored bars with
        // theme CSS variables; the chart palette is now static. Drop the stale
        // copy so it reseeds below with fixed hex colors.
        if (budgetByBu != null && budgetByBu.QueryString!.Contains("var(--color-"))
        {
            if(budgetByBu.ReportRoleAssignmentList != null &&budgetByBu.ReportRoleAssignmentList.Count > 0)
            {
                context.ReportRoleAssignment.RemoveRange(budgetByBu.ReportRoleAssignmentList);
            }
            if (budgetByBu.ReportQueryFilterList != null && budgetByBu.ReportQueryFilterList.Count > 0)
            {
                context.ReportQueryFilter.RemoveRange(budgetByBu.ReportQueryFilterList);
            }
 
            context.Report.Remove(budgetByBu);
            await context.SaveChangesAsync();
            budgetByBu = null;
        }

        if (budgetByBu == null)
        {
            // Widget 2: budget-vs-target progress bars. Not a chart - Custom Html
            // with every presentational value (percent widths, colors, badge class,
            // delta label) precomputed in the query so the template is dumb
            // substitution. Percentages share one scale so bar lengths compare
            // across BUs; the blue tick marks each BU's budget target.
            var budgetByBuReportId = Guid.NewGuid().ToString();
            context.Report.Add(new ReportState()
            {
                Id = budgetByBuReportId,
                ReportName = "Budget Performance by BU",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.CustomHtml,
                IsDistinct = false,
                QueryString = @"SELECT [BusinessUnit], [ActualM], [DeltaLabel], [BadgeClass], [FillPct], [TargetPct], [BarColor]
                    FROM (VALUES
                        ('Filinvest Land', 312, '-28M', 'badge-soft-success', 92, 100, '#10b981'),
                        ('Filinvest Alabang', 295, '+15M', 'badge-soft-danger', 87, 82, '#ef4444'),
                        ('FDC Utilities', 198, '-22M', 'badge-soft-success', 58, 65, '#10b981'),
                        ('Countrywide Water', 172, '-8M', 'badge-soft-success', 51, 53, '#10b981'),
                        ('Chroma Hospitality', 115, '-5M', 'badge-soft-success', 34, 35, '#10b981'),
                        ('SharePro', 88, '-2M', 'badge-soft-success', 26, 27, '#10b981')
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
                            <div style="position: absolute; left: {TargetPct}%; top: -2px; height: 16px; width: 3px; background-color: #2563eb; border-radius: 2px;"></div>
                        </div>
                    </div>
                    <!--{/foreach}-->
                    <div style="display: flex; gap: 18px; margin-top: 4px; font-size: var(--font-size-dense); color: var(--text-muted);">
                        <span><span style="display: inline-block; width: 10px; height: 10px; border-radius: 50%; background-color: #10b981; margin-right: 5px;"></span>Within budget</span>
                        <span><span style="display: inline-block; width: 10px; height: 10px; border-radius: 50%; background-color: #ef4444; margin-right: 5px;"></span>Over budget</span>
                        <span><span style="display: inline-block; width: 3px; height: 12px; background-color: #2563eb; margin-right: 5px;"></span>Budget target</span>
                    </div>
                </div>
                """,
                Sequence = 3,
                SpanWidth = 33,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        ReportId = budgetByBuReportId,
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
        }
        // Executive-attention table (Red & Amber projects). Custom Html - the
        // built-in Table chart type renders via DataTables and can't compose
        // two-line cells or status badges. All presentational values (badge
        // classes/labels) are precomputed in the static query; swap the VALUES
        // block for a real query over Project once live data exists.
        var executiveAttention = await context.Report.FirstOrDefaultAsync(e => e.ReportName == "Projects Requiring Executive Attention");
        if (executiveAttention == null)
        {
            var executiveAttentionReportId = Guid.NewGuid().ToString();
            context.Report.Add(new ReportState()
            {
                Id = executiveAttentionReportId,
                ReportName = "Projects Requiring Executive Attention",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.CustomHtml,
                IsDistinct = false,
                QueryString = @"SELECT [Id], [Project], [Description], [BU], [PM], [HealthLabel], [HealthClass], [BudgetLabel], [BudgetClass], [ScheduleLabel], [ScheduleClass], [LastReview]
                    FROM (VALUES
                        ('CPE-001', 'Customer Portal Enhancement', 'Critical blockers on API integration. Vendor SLA breached.', 'FLI', 'Emil', 'Red', 'badge-soft-danger', 'Overbudget', 'badge-soft-danger', 'Delayed', 'badge-soft-muted', 'Jul 14, 2026'),
                        ('ESU-003', 'ERP System Upgrade', 'Resource constraints delaying test phase. Risk to go-live.', 'FDCU', 'Gilbert', 'Amber', 'badge-soft-warning', 'On Track', 'badge-soft-success', 'Delayed', 'badge-soft-muted', 'Jul 13, 2026'),
                        ('DWM-005', 'Data Warehouse Modernisation', '3rd-party data pipeline blocked. Escalated to vendor.', 'FAI', 'Von', 'Amber', 'badge-soft-warning', 'At Risk', 'badge-soft-warning', 'On Track', 'badge-soft-success', 'Jul 12, 2026'),
                        ('MAD-007', 'Mobile App Development', 'Design sign-off pending. Dev team on hold.', 'CHI', 'Patrick', 'Red', 'badge-soft-danger', 'Overbudget', 'badge-soft-danger', 'Delayed', 'badge-soft-muted', 'Jul 11, 2026'),
                        ('HSSP-008', 'HR Self-Service Portal', 'Payroll module integration delayed. Vendor escalation ongoing.', 'SPI', 'Rey', 'Amber', 'badge-soft-warning', 'At Risk', 'badge-soft-warning', 'On Track', 'badge-soft-success', 'Jul 10, 2026')
                    ) v([Id], [Project], [Description], [BU], [PM], [HealthLabel], [HealthClass], [BudgetLabel], [BudgetClass], [ScheduleLabel], [ScheduleClass], [LastReview]);",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                HtmlTemplate = """
                <div style="overflow-x: auto;">
                    <table style="width: 100%; border-collapse: collapse; text-align: left;">
                        <thead>
                            <tr style="background-color: #f8f9fb; border-bottom: 1px solid #e9ecf2;">
                                <th style="padding: 10px 12px; font-family: var(--font-secondary); font-size: 0.7rem; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748b; white-space: nowrap;">ID</th>
                                <th style="padding: 10px 12px; font-family: var(--font-secondary); font-size: 0.7rem; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748b;">Project</th>
                                <th style="padding: 10px 12px; font-family: var(--font-secondary); font-size: 0.7rem; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748b; white-space: nowrap;">BU</th>
                                <th style="padding: 10px 12px; font-family: var(--font-secondary); font-size: 0.7rem; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748b; white-space: nowrap;">PM</th>
                                <th style="padding: 10px 12px; font-family: var(--font-secondary); font-size: 0.7rem; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748b; white-space: nowrap;">Health</th>
                                <th style="padding: 10px 12px; font-family: var(--font-secondary); font-size: 0.7rem; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748b; white-space: nowrap;">Budget</th>
                                <th style="padding: 10px 12px; font-family: var(--font-secondary); font-size: 0.7rem; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748b; white-space: nowrap;">Schedule</th>
                                <th style="padding: 10px 12px; font-family: var(--font-secondary); font-size: 0.7rem; font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; color: #64748b; white-space: nowrap;">Last Review</th>
                                <th style="padding: 10px 12px;"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <!--{#foreach Table}-->
                            <tr style="border-bottom: 1px solid #eef1f6;">
                                <td style="padding: 14px 12px; font-family: var(--font-secondary); font-size: var(--font-size-dense); color: #64748b; white-space: nowrap;">{Id}</td>
                                <td style="padding: 14px 12px; min-width: 220px;">
                                    <div style="font-weight: 600; color: #0f172a; font-size: var(--font-size-base);">{Project}</div>
                                    <div style="color: #64748b; font-size: var(--font-size-dense); margin-top: 2px; max-width: 320px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">{Description}</div>
                                </td>
                                <td style="padding: 14px 12px; color: #64748b; font-size: var(--font-size-base); white-space: nowrap;">{BU}</td>
                                <td style="padding: 14px 12px; color: #0f172a; font-size: var(--font-size-base); white-space: nowrap;">{PM}</td>
                                <td style="padding: 14px 12px; white-space: nowrap;"><span class="badge-soft {HealthClass}">&#9679;&nbsp;{HealthLabel}</span></td>
                                <td style="padding: 14px 12px; white-space: nowrap;"><span class="badge-soft {BudgetClass}">{BudgetLabel}</span></td>
                                <td style="padding: 14px 12px; white-space: nowrap;"><span class="badge-soft {ScheduleClass}">{ScheduleLabel}</span></td>
                                <td style="padding: 14px 12px; font-family: var(--font-secondary); font-size: var(--font-size-dense); color: #64748b; white-space: nowrap;">{LastReview}</td>
                                <td style="padding: 14px 12px; text-align: right;"><i class="fas fa-chevron-right" style="color: #94a3b8;"></i></td>
                            </tr>
                            <!--{/foreach}-->
                        </tbody>
                    </table>
                </div>
                """,
                Sequence = 4,
                SpanWidth = 100,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        ReportId = executiveAttentionReportId,
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
        }
        // Weekly reporting compliance gauge. Custom Html with an inline SVG
        // progress ring (Chart.js doughnuts have no center text or rounded
        // caps). RingDash is precomputed: compliance% x ring circumference
        // (2 x pi x r, r=41 -> 257.6). Static values; swap for real counts
        // over ProjectHistory/status submissions once live data exists.
        var reportingCompliance = await context.Report.FirstOrDefaultAsync(e => e.ReportName == "Weekly Reporting Compliance");
        if (reportingCompliance == null)
        {
            var reportingComplianceReportId = Guid.NewGuid().ToString();
            context.Report.Add(new ReportState()
            {
                Id = reportingComplianceReportId,
                ReportName = "Weekly Reporting Compliance",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.CustomHtml,
                IsDistinct = false,
                QueryString = @"SELECT 82 AS [CompliancePct], '211.2 257.6' AS [RingDash], 96 AS [Submitted], 18 AS [Outstanding], 8 AS [Overdue];",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                HtmlTemplate = """
                <div style="display: flex; align-items: center; justify-content: center; gap: 28px; flex-wrap: wrap; padding: 12px;">
                    <!--{#foreach Table}-->
                    <svg viewBox="0 0 100 100" width="150" height="150" role="img" aria-label="{CompliancePct}% compliance">
                        <circle cx="50" cy="50" r="41" fill="none" stroke="#e8ebf3" stroke-width="9"></circle>
                        <circle cx="50" cy="50" r="41" fill="none" stroke="#10b981" stroke-width="9" stroke-linecap="round" stroke-dasharray="{RingDash}" transform="rotate(-90 50 50)"></circle>
                        <text x="50" y="49" text-anchor="middle" font-size="15" font-weight="700" fill="#10b981" style="font-family: var(--font-primary);">{CompliancePct}%</text>
                        <text x="50" y="61" text-anchor="middle" font-size="7" fill="#94a3b8" style="font-family: var(--font-secondary);">compliance</text>
                    </svg>
                    <div style="min-width: 210px;">
                        <div style="display: flex; align-items: center; gap: 10px; padding: 8px 0; font-size: var(--font-size-base); color: #475569;">
                            <span style="display: inline-block; width: 10px; height: 10px; border-radius: 50%; background-color: #10b981;"></span>
                            <span style="flex: 1 1 auto;">Submitted</span>
                            <strong style="color: #0f172a;">{Submitted}</strong>
                        </div>
                        <div style="display: flex; align-items: center; gap: 10px; padding: 8px 0; font-size: var(--font-size-base); color: #475569;">
                            <span style="display: inline-block; width: 10px; height: 10px; border-radius: 50%; background-color: #f59e0b;"></span>
                            <span style="flex: 1 1 auto;">Outstanding</span>
                            <strong style="color: #0f172a;">{Outstanding}</strong>
                        </div>
                        <div style="display: flex; align-items: center; gap: 10px; padding: 8px 0; font-size: var(--font-size-base); color: #475569;">
                            <span style="display: inline-block; width: 10px; height: 10px; border-radius: 50%; background-color: #ef4444;"></span>
                            <span style="flex: 1 1 auto;">Overdue</span>
                            <strong style="color: #0f172a;">{Overdue}</strong>
                        </div>
                    </div>
                    <!--{/foreach}-->
                </div>
                """,
                Sequence = 5,
                SpanWidth = 33,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        ReportId = reportingComplianceReportId,
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
        }

        // Top Risks list (severity pill + risk + project). Custom Html; static
        // rows - swap the VALUES block for a real risks query once the data
        // model exists.
        var topRisks = await context.Report.FirstOrDefaultAsync(e => e.ReportName == "Top Risks");
        if (topRisks == null)
        {
            var topRisksReportId = Guid.NewGuid().ToString();
            context.Report.Add(new ReportState()
            {
                Id = topRisksReportId,
                ReportName = "Top Risks",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.CustomHtml,
                IsDistinct = false,
                QueryString = @"SELECT [Severity], [SeverityClass], [Risk], [Project]
                    FROM (VALUES
                        ('HIGH', 'badge-soft-danger', 'Integration conflict with legacy system', 'Customer Portal'),
                        ('HIGH', 'badge-soft-danger', '300+ service disruption risk (Oct 2024)', 'ERP Upgrade'),
                        ('HIGH', 'badge-soft-danger', 'Data migration dependency on vendor', 'Data Warehouse'),
                        ('MEDIUM', 'badge-soft-warning', 'Data migration and quality issues', 'Mobile App Dev')
                    ) v([Severity], [SeverityClass], [Risk], [Project]);",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                HtmlTemplate = """
                <div style="width: 100%;">
                    <!--{#foreach Table}-->
                    <div style="display: flex; gap: 12px; padding: 10px 4px; align-items: flex-start;">
                        <span class="badge-soft {SeverityClass}" style="font-family: var(--font-secondary); flex-shrink: 0;">{Severity}</span>
                        <div style="min-width: 0;">
                            <div style="font-size: var(--font-size-base); font-weight: 600; color: #0f172a;">{Risk}</div>
                            <div style="font-family: var(--font-secondary); font-size: var(--font-size-dense); color: #94a3b8; margin-top: 2px;">{Project}</div>
                        </div>
                    </div>
                    <!--{/foreach}-->
                    <div style="text-align: right; padding: 8px 4px 0;"><a href="#" style="color: #2563eb; font-family: var(--font-secondary); font-size: var(--font-size-base); text-decoration: none;">View Report</a></div>
                </div>
                """,
                Sequence = 6,
                SpanWidth = 33,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        ReportId = topRisksReportId,
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
        }

        // Upcoming Milestones list (calendar date block + title + owner +
        // days-remaining chip). Custom Html; static rows with all colors and
        // chips precomputed - urgent (<10d) rows get the red date and chip.
        var upcomingMilestones = await context.Report.FirstOrDefaultAsync(e => e.ReportName == "Upcoming Milestones");
        if (upcomingMilestones == null)
        {
            var upcomingMilestonesReportId = Guid.NewGuid().ToString();
            context.Report.Add(new ReportState()
            {
                Id = upcomingMilestonesReportId,
                ReportName = "Upcoming Milestones",
                QueryType = Core.Constants.QueryType.TSql,
                ReportOrChartType = Core.Constants.ReportChartType.CustomHtml,
                IsDistinct = false,
                QueryString = @"SELECT [Month], [Day], [MonthColor], [DayColor], [Title], [Detail], [ChipLabel], [ChipClass]
                    FROM (VALUES
                        ('MAY', '20', '#ef4444', '#ef4444', 'Phase 3 Deployment', 'Customer Portal Enhancement · Emil', '3d', 'badge-soft-danger'),
                        ('MAY', '25', '#ef4444', '#ef4444', 'Data Migration Complete', 'ERP System Upgrade · Gilbert', '8d', 'badge-soft-danger'),
                        ('JUN', '3', '#94a3b8', '#0f172a', 'User Acceptance Testing', 'Data Warehouse Modernisation · Von', '17d', 'badge-soft-muted'),
                        ('JUN', '5', '#94a3b8', '#0f172a', 'ERP Acceptance Sign-off', 'ERP System Upgrade · Rey', '19d', 'badge-soft-muted'),
                        ('JUN', '10', '#94a3b8', '#0f172a', 'Mobile App Beta Release', 'Mobile App Development · Patrick', '24d', 'badge-soft-muted')
                    ) v([Month], [Day], [MonthColor], [DayColor], [Title], [Detail], [ChipLabel], [ChipClass]);",
                DisplayOnDashboard = true,
                DisplayOnReportModule = false,
                HtmlTemplate = """
                <div style="width: 100%;">
                    <!--{#foreach Table}-->
                    <div style="display: flex; align-items: center; gap: 14px; padding: 12px 4px; border-bottom: 1px solid #eef1f6;">
                        <div style="text-align: center; min-width: 38px; flex-shrink: 0;">
                            <div style="font-family: var(--font-secondary); font-size: 0.65rem; font-weight: 700; letter-spacing: 0.06em; color: {MonthColor};">{Month}</div>
                            <div style="font-size: 1.15rem; font-weight: 700; color: {DayColor}; line-height: 1.1;">{Day}</div>
                        </div>
                        <div style="flex: 1 1 auto; min-width: 0;">
                            <div style="font-size: var(--font-size-base); font-weight: 600; color: #0f172a;">{Title}</div>
                            <div style="font-size: var(--font-size-dense); color: #94a3b8; margin-top: 2px;">{Detail}</div>
                        </div>
                        <span class="badge-soft {ChipClass}" style="font-family: var(--font-secondary); flex-shrink: 0;">{ChipLabel}</span>
                    </div>
                    <!--{/foreach}-->
                    <div style="text-align: right; padding: 10px 4px 0;"><a href="#" style="color: #2563eb; font-family: var(--font-secondary); font-size: var(--font-size-base); text-decoration: none;">View all</a></div>
                </div>
                """,
                Sequence = 7,
                SpanWidth = 33,
                ReportRoleAssignmentList = [
                    new ReportRoleAssignmentState()
                    {
                        ReportId = upcomingMilestonesReportId,
                        Id = Guid.NewGuid().ToString(),
                        RoleName = Core.Constants.Roles.Admin
                    }
                ]
            });
            await context.SaveChangesAsync();
        }

        

      
    }
}
