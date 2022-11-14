using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmsiScanner.Common.Threading {
    //public delegate void BackgroundWorkerEventHandler(BackgroundWorker worker);

    public abstract class BackgroundWorker {
        //public event BackgroundWorkerEventHandler OnWorkerStarted;
        //public event BackgroundWorkerEventHandler OnWorkerStopped;

        public BackgroundWorkerStatus Status {
             get {
                lock (this._lock) {
                    return this._status;
                }
            }
        }

        public void Start() {
            lock (this._lock) {
                if(this._status != BackgroundWorkerStatus.NotStarted) {
                    return;
                }

                try {
                    this._status = BackgroundWorkerStatus.Starting;

                    this._thread = new Thread(this.RunHelper);

                    this._thread.Start();
                } catch(Exception ex) {
                    this._status = BackgroundWorkerStatus.Starting;
                    throw ex;
                }
            }
        }

        public void Stop() {
            lock (this._lock) {
                if (this._status != BackgroundWorkerStatus.Running) {
                    return;
                }

                this._source.Cancel();
            }
        }

        public void Wait() {
            if (this.Status == BackgroundWorkerStatus.NotStarted) {
                throw new InvalidOperationException("Cannot wait on a worker that has not been started.");
            }

            while(this.Status != BackgroundWorkerStatus.Complete) {
                Thread.Sleep(250);
            }
        }

        public void Wait(int milliseconds) {
            if (this.Status == BackgroundWorkerStatus.NotStarted) {
                throw new InvalidOperationException("Cannot wait on a worker that has not been started.");
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (this.Status != BackgroundWorkerStatus.Complete && watch.ElapsedMilliseconds < milliseconds) {
                Thread.Sleep(100);
            }
        }

        private void RunHelper() {
            lock (this._lock) {
                this._status = BackgroundWorkerStatus.Starting;
            }
            this.OnStarting();

            lock (this._lock) {
                this._status = BackgroundWorkerStatus.Running;
            }
            this.Run(this._source.Token);

            lock (this._lock) {
                this._status = BackgroundWorkerStatus.Stopping;
            }
            this.OnStopping();

            lock (this._lock) {
                this._status = BackgroundWorkerStatus.Complete;
            }
        }

        protected virtual void OnStarting() { }

        protected abstract void Run(CancellationToken token);

        protected virtual void OnStopping() { }

        private Thread _thread = null;
        private object _lock = new object();
        private BackgroundWorkerStatus _status = BackgroundWorkerStatus.NotStarted;
        private CancellationTokenSource _source = new CancellationTokenSource();
    }
}
