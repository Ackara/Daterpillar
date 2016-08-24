﻿using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.Arguments;
using Gigobyte.Daterpillar.Compare;
using System.Data;

namespace Gigobyte.Daterpillar.Commands
{
    [VerbLink(SyncVerb.Name)]
    public class SyncCommand : CommandBase
    {
        public override int Execute(object args)
        {
            var options = (SyncVerb)args;
            var source = _aggregatorFactory.CreateInstance(options.Platform, GetConnection(options.Platform, options.Source));
            var target = _aggregatorFactory.CreateInstance(options.Platform, GetConnection(options.Platform, options.Target));

            ComparisonReport report = new SchemaComparer().GenerateReport(source, target);
            switch (report.Summary)
            {
                case ComparisonReportConclusions.NotEqual:
                    // TODO: Add code here
                    break;

                case ComparisonReportConclusions.Equal:
                    // TODO: Add code here
                    break;

                case ComparisonReportConclusions.SourceEmpty:
                    // TODO: Add code here
                    break;

                case ComparisonReportConclusions.TargetEmpty:
                    // TODO: Add code here
                    break;
            }

            return ExitCode.Success;
        }

        #region Private Members

        private readonly SchemaAggregatorFactory _aggregatorFactory = new SchemaAggregatorFactory();
        
        #endregion Private Members
    }
}