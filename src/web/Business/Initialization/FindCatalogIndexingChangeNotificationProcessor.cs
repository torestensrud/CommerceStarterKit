using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EPiServer.Events.ChangeNotification;
using EPiServer.Logging;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Search;

namespace OxxCommerceStarterKit.Web.Business.Initialization
{
    public class FindCatalogIndexingChangeNotificationProcessor : IChangeProcessor<string>
    {
        public FindCatalogIndexingChangeNotificationProcessor()
        {
            _log.Debug("FindCatalogIndexingChangeNotificationProcessor initialized.");
        }

        private class CatalogEntryChangeListener : IChangeListener<CatalogEntryChange, string>
        {
            protected static ILogger _log = LogManager.GetLogger();

            public IEnumerable<string> NotifyChange(CatalogEntryChange before, CatalogEntryChange after)
            {
                if (before != null)
                {
                    _log.Debug("NotifyChange before: {0}", before.GetQueueableString());
                    yield return before.GetQueueableString();
                    if (after != null && !before.Equals(after))
                    {
                        _log.Debug("NotifyChange after: {0}", after.GetQueueableString());
                        yield return after.GetQueueableString();
                    }
                }
                else if (after != null)
                {
                    _log.Debug("NotifyChange after: {0}", after.GetQueueableString());
                    yield return after.GetQueueableString();
                }
            }
        }

        protected static ILogger _log = LogManager.GetLogger();
        private const string _displayName = "Find Catalog Indexer";
        private static readonly Guid _processorId = new Guid("ea403de2-c91d-4675-a82e-ac087ea368de");
        private static readonly IChangeListener _changeListener = new CatalogEntryChangeListener();
        private static readonly TimeSpan _retryInterval = TimeSpan.FromSeconds(30);
        private static readonly int _maxBatchSize = 2;



        public static Guid ProcessorId
        {
            get { return _processorId; }
        }

        Guid IChangeProcessor.ProcessorId
        {
            get { return _processorId; }
        }

        public string Name
        {
            get { return _displayName; }
        }

        public int MaxBatchSize
        {
            get
            {
                _log.Debug("MaxBatchSize: {0}", _maxBatchSize);
                return _maxBatchSize;
            }
        }

        public int MaxRetryCount
        {
            get
            {
                _log.Debug("MaxRetryCount: 10");
                return 10;
            }
        }

        public TimeSpan RetryInterval
        {
            get
            {
                _log.Debug("RetryInterval: 30 sec");
                return _retryInterval;
            }
        }

        public IEnumerable<IChangeListener> Listeners
        {
            get { yield return _changeListener; }
        }


        public bool RecoverConsistency(IRecoveryContext recoveryContext)
        {
            _log.Debug("RecoverConsistency called");
            // recoveryContext.SetActivity();
            recoveryContext.SetProgress(1, 1);

            return true;
        }


        public bool ProcessItems(IEnumerable<string> items, CancellationToken cancellationToken)
        {
            _log.Debug("ProcessItems called");

            foreach (string item in items)
            {
                _log.Debug("Process: {0}", item);
            }

            var result = true;

            return result;
        }
    }
}
