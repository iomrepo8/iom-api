using IOM.Services;
using System;
using System.Threading;

namespace IOM.Hubs.Util
{
    public class TimerManager : IDisposable
    {
        private Timer _timer;
        private AutoResetEvent _autoResetEvent;
        private Action<object> _action;
        private int _delay = 1000;
        private int _interval = 10000;
        private string _netUserId = "";
        public DateTime TimerStarted { get; }
        public TimerManager(string netUserId, Action<object> action)
        {
            _netUserId = netUserId;
            _action = action;
            _autoResetEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoResetEvent, _delay, _interval);
            TimerStarted = DateTime.Now;
        }
        public void Execute(object stateInfo) => _action(_netUserId);

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}