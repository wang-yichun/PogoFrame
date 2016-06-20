// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace uFrame.ExampleProject {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.StateMachine;
    
    
    public class LevelSM : Invert.StateMachine.StateMachine {
        
        private Invert.StateMachine.StateMachineTrigger _Level_LoadingFinished;
        
        private Invert.StateMachine.StateMachineTrigger _Level_Reset;
        
        private Invert.StateMachine.StateMachineTrigger _Level_Close;
        
        private Invert.StateMachine.StateMachineTrigger _Level_HotReload;
        
        private Invert.StateMachine.StateMachineTrigger _Level_Run;
        
        private Level_Loading _Level_Loading;
        
        private Level_AssetsStandby _Level_AssetsStandby;
        
        private Level_Closing _Level_Closing;
        
        private Level_Reloading _Level_Reloading;
        
        private Level_Running _Level_Running;
        
        public LevelSM(uFrame.MVVM.ViewModel vm, string propertyName) : 
                base(vm, propertyName) {
        }
        
        public LevelSM() : 
                base(null, string.Empty) {
        }
        
        public override Invert.StateMachine.State StartState {
            get {
                return this.Level_Loading;
            }
        }
        
        public virtual Invert.StateMachine.StateMachineTrigger Level_LoadingFinished {
            get {
                if (this._Level_LoadingFinished == null) {
                    this._Level_LoadingFinished = new StateMachineTrigger(this , "Level_LoadingFinished");
                }
                return _Level_LoadingFinished;
            }
            set {
                _Level_LoadingFinished = value;
            }
        }
        
        public virtual Invert.StateMachine.StateMachineTrigger Level_Reset {
            get {
                if (this._Level_Reset == null) {
                    this._Level_Reset = new StateMachineTrigger(this , "Level_Reset");
                }
                return _Level_Reset;
            }
            set {
                _Level_Reset = value;
            }
        }
        
        public virtual Invert.StateMachine.StateMachineTrigger Level_Close {
            get {
                if (this._Level_Close == null) {
                    this._Level_Close = new StateMachineTrigger(this , "Level_Close");
                }
                return _Level_Close;
            }
            set {
                _Level_Close = value;
            }
        }
        
        public virtual Invert.StateMachine.StateMachineTrigger Level_HotReload {
            get {
                if (this._Level_HotReload == null) {
                    this._Level_HotReload = new StateMachineTrigger(this , "Level_HotReload");
                }
                return _Level_HotReload;
            }
            set {
                _Level_HotReload = value;
            }
        }
        
        public virtual Invert.StateMachine.StateMachineTrigger Level_Run {
            get {
                if (this._Level_Run == null) {
                    this._Level_Run = new StateMachineTrigger(this , "Level_Run");
                }
                return _Level_Run;
            }
            set {
                _Level_Run = value;
            }
        }
        
        public virtual Level_Loading Level_Loading {
            get {
                if (this._Level_Loading == null) {
                    this._Level_Loading = new Level_Loading();
                }
                return _Level_Loading;
            }
            set {
                _Level_Loading = value;
            }
        }
        
        public virtual Level_AssetsStandby Level_AssetsStandby {
            get {
                if (this._Level_AssetsStandby == null) {
                    this._Level_AssetsStandby = new Level_AssetsStandby();
                }
                return _Level_AssetsStandby;
            }
            set {
                _Level_AssetsStandby = value;
            }
        }
        
        public virtual Level_Closing Level_Closing {
            get {
                if (this._Level_Closing == null) {
                    this._Level_Closing = new Level_Closing();
                }
                return _Level_Closing;
            }
            set {
                _Level_Closing = value;
            }
        }
        
        public virtual Level_Reloading Level_Reloading {
            get {
                if (this._Level_Reloading == null) {
                    this._Level_Reloading = new Level_Reloading();
                }
                return _Level_Reloading;
            }
            set {
                _Level_Reloading = value;
            }
        }
        
        public virtual Level_Running Level_Running {
            get {
                if (this._Level_Running == null) {
                    this._Level_Running = new Level_Running();
                }
                return _Level_Running;
            }
            set {
                _Level_Running = value;
            }
        }
        
        public override void Compose(System.Collections.Generic.List<Invert.StateMachine.State> states) {
            base.Compose(states);
            Level_Loading.Level_LoadingFinished = new StateTransition("Level_LoadingFinished", Level_Loading, Level_AssetsStandby);
            Transitions.Add(Level_Loading.Level_LoadingFinished);
            Level_Loading.AddTrigger(Level_LoadingFinished, Level_Loading.Level_LoadingFinished);
            Level_Loading.StateMachine = this;
            states.Add(Level_Loading);
            Level_AssetsStandby.Level_Close = new StateTransition("Level_Close", Level_AssetsStandby, Level_Closing);
            Transitions.Add(Level_AssetsStandby.Level_Close);
            Level_AssetsStandby.Level_HotReload = new StateTransition("Level_HotReload", Level_AssetsStandby, Level_Reloading);
            Transitions.Add(Level_AssetsStandby.Level_HotReload);
            Level_AssetsStandby.Level_Run = new StateTransition("Level_Run", Level_AssetsStandby, Level_Running);
            Transitions.Add(Level_AssetsStandby.Level_Run);
            Level_AssetsStandby.AddTrigger(Level_Close, Level_AssetsStandby.Level_Close);
            Level_AssetsStandby.AddTrigger(Level_HotReload, Level_AssetsStandby.Level_HotReload);
            Level_AssetsStandby.AddTrigger(Level_Run, Level_AssetsStandby.Level_Run);
            Level_AssetsStandby.StateMachine = this;
            states.Add(Level_AssetsStandby);
            Level_Closing.Level_Reset = new StateTransition("Level_Reset", Level_Closing, Level_Loading);
            Transitions.Add(Level_Closing.Level_Reset);
            Level_Closing.AddTrigger(Level_Reset, Level_Closing.Level_Reset);
            Level_Closing.StateMachine = this;
            states.Add(Level_Closing);
            Level_Reloading.Level_LoadingFinished = new StateTransition("Level_LoadingFinished", Level_Reloading, Level_AssetsStandby);
            Transitions.Add(Level_Reloading.Level_LoadingFinished);
            Level_Reloading.AddTrigger(Level_LoadingFinished, Level_Reloading.Level_LoadingFinished);
            Level_Reloading.StateMachine = this;
            states.Add(Level_Reloading);
            Level_Running.Level_HotReload = new StateTransition("Level_HotReload", Level_Running, Level_Reloading);
            Transitions.Add(Level_Running.Level_HotReload);
            Level_Running.Level_Close = new StateTransition("Level_Close", Level_Running, Level_Closing);
            Transitions.Add(Level_Running.Level_Close);
            Level_Running.AddTrigger(Level_HotReload, Level_Running.Level_HotReload);
            Level_Running.AddTrigger(Level_Close, Level_Running.Level_Close);
            Level_Running.StateMachine = this;
            states.Add(Level_Running);
        }
    }
    
    public class Level_Loading : Invert.StateMachine.State {
        
        private Invert.StateMachine.StateTransition _Level_LoadingFinished;
        
        public Invert.StateMachine.StateTransition Level_LoadingFinished {
            get {
                return _Level_LoadingFinished;
            }
            set {
                _Level_LoadingFinished = value;
            }
        }
        
        public override string Name {
            get {
                return "Level_Loading";
            }
        }
        
        public virtual void Level_LoadingFinishedTransition() {
            this.Transition(this.Level_LoadingFinished);
        }
    }
    
    public class Level_AssetsStandby : Invert.StateMachine.State {
        
        private Invert.StateMachine.StateTransition _Level_Close;
        
        private Invert.StateMachine.StateTransition _Level_HotReload;
        
        private Invert.StateMachine.StateTransition _Level_Run;
        
        public Invert.StateMachine.StateTransition Level_Close {
            get {
                return _Level_Close;
            }
            set {
                _Level_Close = value;
            }
        }
        
        public Invert.StateMachine.StateTransition Level_HotReload {
            get {
                return _Level_HotReload;
            }
            set {
                _Level_HotReload = value;
            }
        }
        
        public Invert.StateMachine.StateTransition Level_Run {
            get {
                return _Level_Run;
            }
            set {
                _Level_Run = value;
            }
        }
        
        public override string Name {
            get {
                return "Level_AssetsStandby";
            }
        }
        
        public virtual void Level_CloseTransition() {
            this.Transition(this.Level_Close);
        }
        
        public virtual void Level_HotReloadTransition() {
            this.Transition(this.Level_HotReload);
        }
        
        public virtual void Level_RunTransition() {
            this.Transition(this.Level_Run);
        }
    }
    
    public class Level_Closing : Invert.StateMachine.State {
        
        private Invert.StateMachine.StateTransition _Level_Reset;
        
        public Invert.StateMachine.StateTransition Level_Reset {
            get {
                return _Level_Reset;
            }
            set {
                _Level_Reset = value;
            }
        }
        
        public override string Name {
            get {
                return "Level_Closing";
            }
        }
        
        public virtual void Level_ResetTransition() {
            this.Transition(this.Level_Reset);
        }
    }
    
    public class Level_Reloading : Invert.StateMachine.State {
        
        private Invert.StateMachine.StateTransition _Level_LoadingFinished;
        
        public Invert.StateMachine.StateTransition Level_LoadingFinished {
            get {
                return _Level_LoadingFinished;
            }
            set {
                _Level_LoadingFinished = value;
            }
        }
        
        public override string Name {
            get {
                return "Level_Reloading";
            }
        }
        
        public virtual void Level_LoadingFinishedTransition() {
            this.Transition(this.Level_LoadingFinished);
        }
    }
    
    public class Level_Running : Invert.StateMachine.State {
        
        private Invert.StateMachine.StateTransition _Level_HotReload;
        
        private Invert.StateMachine.StateTransition _Level_Close;
        
        public Invert.StateMachine.StateTransition Level_HotReload {
            get {
                return _Level_HotReload;
            }
            set {
                _Level_HotReload = value;
            }
        }
        
        public Invert.StateMachine.StateTransition Level_Close {
            get {
                return _Level_Close;
            }
            set {
                _Level_Close = value;
            }
        }
        
        public override string Name {
            get {
                return "Level_Running";
            }
        }
        
        public virtual void Level_HotReloadTransition() {
            this.Transition(this.Level_HotReload);
        }
        
        public virtual void Level_CloseTransition() {
            this.Transition(this.Level_Close);
        }
    }
}
