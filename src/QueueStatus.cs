using Sufficit.Asterisk.Manager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    public class QueueStatus
    {
        private int _validEventsCount;
        private readonly string _title;
        private readonly List<QueueAgentStatus> _agents;

        public QueueStatus(string title)
        {
            _title = title;
            _agents = new List<QueueAgentStatus>();
        }

        #region PROPRIEDADES PUBLICAS

        public event Action UpdatedEvent;

        public int Events => _validEventsCount;

        public string Title => _title;

        public IEnumerable<QueueAgentStatus> Agents => _agents;


        public string Strategy { get; set; }
        public int Max { get; set; }
        public int Calls { get; set; }
        public int HoldTime { get; set; }
        public int Completed { get; set; }
        public int Abandoned { get; set; }
        public int ServiceLevel { get; set; }
        public float ServiceLevelPerf { get; set; }
        public int Weight { get; set; }
        public int TalkTime { get; set; }

        public DateTime Updated { get; internal set; }

        #endregion

        public void ReceiveEventHandler(string sender, AMIQueueMemberStatusEvent eventObj)
        {
            if (eventObj.Queue != _title) return;
            _validEventsCount++;

            QueueAgentStatus status = _agents.Find(s => s.Interface == eventObj.Interface);
            if (status == null)
            {
                status = new QueueAgentStatus(eventObj.Interface);
                _agents.Add(status);
            }

            if (eventObj.DateReceived > Updated)
            {
                status.Updated = eventObj.DateReceived;
                status.Title = eventObj.MemberName;
                status.Membership = eventObj.Membership;
                status.Penalty = eventObj.Penalty;
                status.CallsTaken = eventObj.CallsTaken;
                status.LastCall = eventObj.LastCall;
                status.Status = eventObj.Status;
                status.Paused = eventObj.Paused;
                status.InCall = eventObj.InCall;
                status.PausedReason = eventObj.PausedReason;
                status.LastPause = eventObj.LastPause;
                status.WrapUpTime = eventObj.WrapUpTime;

                UpdatedEvent?.Invoke();
            }
        }

        public void ReceiveEventHandler(string sender, AMIQueueParamsEvent eventObj)
        {
            if (eventObj.Queue != _title) return;
            _validEventsCount++;

            if (eventObj.DateReceived > Updated)
            {
                Updated = eventObj.DateReceived;
                Strategy = eventObj.Strategy;
                Max = eventObj.Max;
                Calls = eventObj.Calls;
                HoldTime = eventObj.Holdtime;
                Completed = eventObj.Completed;
                Abandoned = eventObj.Abandoned;
                ServiceLevel = eventObj.ServiceLevel;
                ServiceLevelPerf = eventObj.ServiceLevelPerf;
                Weight = eventObj.Weight;
                TalkTime = eventObj.TalkTime;

                UpdatedEvent?.Invoke();
            }
        }

        public void ReceiveEventHandler(string sender, AMIQueueMemberEvent eventObj)
        {
            if (eventObj.Queue != _title) return;
            _validEventsCount++;

            QueueAgentStatus status = _agents.Find(s => s.Interface == eventObj.Location);
            if (status == null)
            {
                status = new QueueAgentStatus(eventObj.Location);
                _agents.Add(status);
            }

            if (eventObj.DateReceived > status.Updated)
            {
                status.Updated = eventObj.DateReceived;
                status.Title = eventObj.Name;
                status.Membership = eventObj.Membership;
                status.Penalty = eventObj.Penalty;
                status.CallsTaken = eventObj.CallsTaken;
                status.LastCall = eventObj.LastCall;
                status.Status = eventObj.Status;
                status.Paused = eventObj.Paused;
                status.InCall = eventObj.InCall;
                status.PausedReason = eventObj.PausedReason;
                status.LastPause = eventObj.LastPause;
                status.WrapUpTime = eventObj.WrapUpTime;

                UpdatedEvent?.Invoke();
            }
        }
    }
}
