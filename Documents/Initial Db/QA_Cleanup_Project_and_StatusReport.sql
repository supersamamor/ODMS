use odms_qa
/* =====================================================================
   QA CLEANUP - Projects + Status Reports
   ---------------------------------------------------------------------
   Wipes ALL Project and Status Report data so both can be re-uploaded
   fresh, and resets the auto-generated ProjectId counter so codes restart
   at A-...-00001.

   Run on the QA database ONLY. It is wrapped in a transaction: run it,
   review the row counts in the Messages pane, then COMMIT (or ROLLBACK if
   anything looks wrong).

   Deletes are ordered child -> parent to satisfy foreign keys.
   Reference/master data (BusinessUnit, Employee, Milestone catalogue,
   ReportingWeek) is left intact unless you opt in below.
   ===================================================================== */

SET NOCOUNT OFF;
BEGIN TRANSACTION;

------------------------------------------------------------------
-- 1) Status Report child rows
------------------------------------------------------------------
DELETE FROM StatusReportHealthIndicator;      PRINT 'StatusReportHealthIndicator: ' + CAST(@@ROWCOUNT AS varchar(10));
DELETE FROM StatusReportMilestone;            PRINT 'StatusReportMilestone:       ' + CAST(@@ROWCOUNT AS varchar(10));
DELETE FROM StatusReportRiskIssue;            PRINT 'StatusReportRiskIssue:        ' + CAST(@@ROWCOUNT AS varchar(10));

------------------------------------------------------------------
-- 2) Approval workflow rows tied to Status Reports
--    (ApprovalRecord.TableName = 'Status Report', DataId = StatusReport.Id)
------------------------------------------------------------------


------------------------------------------------------------------
-- 3) Status Reports
------------------------------------------------------------------
DELETE FROM StatusReport;                     PRINT 'StatusReport:                 ' + CAST(@@ROWCOUNT AS varchar(10));

------------------------------------------------------------------
-- 4) Project child rows
------------------------------------------------------------------
DELETE FROM TeamMembers;                      PRINT 'TeamMembers:                  ' + CAST(@@ROWCOUNT AS varchar(10));
DELETE FROM ProjectMilestone;                 PRINT 'ProjectMilestone:             ' + CAST(@@ROWCOUNT AS varchar(10));
DELETE FROM RiskIssue;                        PRINT 'RiskIssue (project register): ' + CAST(@@ROWCOUNT AS varchar(10));
DELETE FROM ProjectAttachment;                PRINT 'ProjectAttachment:            ' + CAST(@@ROWCOUNT AS varchar(10));

------------------------------------------------------------------
-- 5) Projects
------------------------------------------------------------------
DELETE FROM Project;                          PRINT 'Project:                      ' + CAST(@@ROWCOUNT AS varchar(10));

------------------------------------------------------------------
-- 6) Reset auto-generated code counters so a fresh upload starts at 1
------------------------------------------------------------------
DELETE FROM SequenceCounter WHERE [Key] LIKE 'ProjectCode:%';
PRINT 'SequenceCounter (ProjectCode):' + CAST(@@ROWCOUNT AS varchar(10));

-- Optional: reset the Risk/Issue register code counter (only if you also
-- uploaded a project risk register that generated R-/I- codes).
-- DELETE FROM SequenceCounter WHERE [Key] LIKE 'RiskIssueCode:%';

------------------------------------------------------------------
-- Optional extras (uncomment only if you want them gone too)
------------------------------------------------------------------
-- Reporting weeks are reference rows and are recreated automatically on the
-- Status Report upload; remove only for a fully clean slate:
-- DELETE FROM ReportingWeek;

-- The Milestone catalogue accumulates entries added by Status Report uploads:
-- DELETE FROM ProjectMilestone;   -- already cleared above
-- DELETE FROM Milestone;          -- clears the milestone master list

------------------------------------------------------------------
-- Review the printed counts above, then choose ONE:
------------------------------------------------------------------
COMMIT TRANSACTION;      -- keep the cleanup
-- ROLLBACK TRANSACTION; -- undo everything (comment out COMMIT above first)
