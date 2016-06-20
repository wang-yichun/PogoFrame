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
    using UniRx;
    using uFrame.Serialization;
    using uFrame.IOC;
    using uFrame.Kernel;
    using uFrame.MVVM;
    using uFrame.ExampleProject;
    
    
    public class MainMenuRootControllerBase : uFrame.MVVM.Controller {
        
        private uFrame.MVVM.IViewModelManager _MainMenuRootViewModelManager;
        
        private MainMenuRootViewModel _MainMenuRoot;
        
        [uFrame.IOC.InjectAttribute("MainMenuRoot")]
        public uFrame.MVVM.IViewModelManager MainMenuRootViewModelManager {
            get {
                return _MainMenuRootViewModelManager;
            }
            set {
                _MainMenuRootViewModelManager = value;
            }
        }
        
        [uFrame.IOC.InjectAttribute("MainMenuRoot")]
        public MainMenuRootViewModel MainMenuRoot {
            get {
                return _MainMenuRoot;
            }
            set {
                _MainMenuRoot = value;
            }
        }
        
        public IEnumerable<MainMenuRootViewModel> MainMenuRootViewModels {
            get {
                return MainMenuRootViewModelManager.OfType<MainMenuRootViewModel>();
            }
        }
        
        public override void Setup() {
            base.Setup();
            // This is called when the controller is created
        }
        
        public override void Initialize(uFrame.MVVM.ViewModel viewModel) {
            base.Initialize(viewModel);
            // This is called when a viewmodel is created
            this.InitializeMainMenuRoot(((MainMenuRootViewModel)(viewModel)));
        }
        
        public virtual MainMenuRootViewModel CreateMainMenuRoot() {
            return ((MainMenuRootViewModel)(this.Create(Guid.NewGuid().ToString())));
        }
        
        public override uFrame.MVVM.ViewModel CreateEmpty() {
            return new MainMenuRootViewModel(this.EventAggregator);
        }
        
        public virtual void InitializeMainMenuRoot(MainMenuRootViewModel viewModel) {
            // This is called when a MainMenuRootViewModel is created
            MainMenuRootViewModelManager.Add(viewModel);
        }
        
        public override void DisposingViewModel(uFrame.MVVM.ViewModel viewModel) {
            base.DisposingViewModel(viewModel);
            MainMenuRootViewModelManager.Remove(viewModel);
        }
    }
    
    public class SubScreenControllerBase : uFrame.MVVM.Controller {
        
        private uFrame.MVVM.IViewModelManager _SubScreenViewModelManager;
        
        private MainMenuRootViewModel _MainMenuRoot;
        
        [uFrame.IOC.InjectAttribute("SubScreen")]
        public uFrame.MVVM.IViewModelManager SubScreenViewModelManager {
            get {
                return _SubScreenViewModelManager;
            }
            set {
                _SubScreenViewModelManager = value;
            }
        }
        
        [uFrame.IOC.InjectAttribute("MainMenuRoot")]
        public MainMenuRootViewModel MainMenuRoot {
            get {
                return _MainMenuRoot;
            }
            set {
                _MainMenuRoot = value;
            }
        }
        
        public IEnumerable<SubScreenViewModel> SubScreenViewModels {
            get {
                return SubScreenViewModelManager.OfType<SubScreenViewModel>();
            }
        }
        
        public override void Setup() {
            base.Setup();
            // This is called when the controller is created
        }
        
        public override void Initialize(uFrame.MVVM.ViewModel viewModel) {
            base.Initialize(viewModel);
            // This is called when a viewmodel is created
            this.InitializeSubScreen(((SubScreenViewModel)(viewModel)));
        }
        
        public virtual SubScreenViewModel CreateSubScreen() {
            return ((SubScreenViewModel)(this.Create(Guid.NewGuid().ToString())));
        }
        
        public override uFrame.MVVM.ViewModel CreateEmpty() {
            return new SubScreenViewModel(this.EventAggregator);
        }
        
        public virtual void InitializeSubScreen(SubScreenViewModel viewModel) {
            // This is called when a SubScreenViewModel is created
            viewModel.Close.Action = this.CloseHandler;
            SubScreenViewModelManager.Add(viewModel);
        }
        
        public override void DisposingViewModel(uFrame.MVVM.ViewModel viewModel) {
            base.DisposingViewModel(viewModel);
            SubScreenViewModelManager.Remove(viewModel);
        }
        
        public virtual void Close(SubScreenViewModel viewModel) {
        }
        
        public virtual void CloseHandler(CloseCommand command) {
            this.Close(command.Sender as SubScreenViewModel);
        }
    }
    
    public class LoginScreenControllerBase : SubScreenController {
        
        private uFrame.MVVM.IViewModelManager _LoginScreenViewModelManager;
        
        [uFrame.IOC.InjectAttribute("LoginScreen")]
        public uFrame.MVVM.IViewModelManager LoginScreenViewModelManager {
            get {
                return _LoginScreenViewModelManager;
            }
            set {
                _LoginScreenViewModelManager = value;
            }
        }
        
        public IEnumerable<LoginScreenViewModel> LoginScreenViewModels {
            get {
                return LoginScreenViewModelManager.OfType<LoginScreenViewModel>();
            }
        }
        
        public override void Setup() {
            base.Setup();
            // This is called when the controller is created
        }
        
        public override void Initialize(uFrame.MVVM.ViewModel viewModel) {
            base.Initialize(viewModel);
            // This is called when a viewmodel is created
            this.InitializeLoginScreen(((LoginScreenViewModel)(viewModel)));
        }
        
        public virtual LoginScreenViewModel CreateLoginScreen() {
            return ((LoginScreenViewModel)(this.Create(Guid.NewGuid().ToString())));
        }
        
        public override uFrame.MVVM.ViewModel CreateEmpty() {
            return new LoginScreenViewModel(this.EventAggregator);
        }
        
        public virtual void InitializeLoginScreen(LoginScreenViewModel viewModel) {
            // This is called when a LoginScreenViewModel is created
            viewModel.Login.Action = this.LoginHandler;
            LoginScreenViewModelManager.Add(viewModel);
        }
        
        public override void DisposingViewModel(uFrame.MVVM.ViewModel viewModel) {
            base.DisposingViewModel(viewModel);
            LoginScreenViewModelManager.Remove(viewModel);
        }
        
        public virtual void Login(LoginScreenViewModel viewModel) {
        }
        
        public virtual void LoginHandler(LoginCommand command) {
            this.Login(command.Sender as LoginScreenViewModel);
        }
    }
    
    public class SettingsScreenControllerBase : SubScreenController {
        
        private uFrame.MVVM.IViewModelManager _SettingsScreenViewModelManager;
        
        [uFrame.IOC.InjectAttribute("SettingsScreen")]
        public uFrame.MVVM.IViewModelManager SettingsScreenViewModelManager {
            get {
                return _SettingsScreenViewModelManager;
            }
            set {
                _SettingsScreenViewModelManager = value;
            }
        }
        
        public IEnumerable<SettingsScreenViewModel> SettingsScreenViewModels {
            get {
                return SettingsScreenViewModelManager.OfType<SettingsScreenViewModel>();
            }
        }
        
        public override void Setup() {
            base.Setup();
            // This is called when the controller is created
        }
        
        public override void Initialize(uFrame.MVVM.ViewModel viewModel) {
            base.Initialize(viewModel);
            // This is called when a viewmodel is created
            this.InitializeSettingsScreen(((SettingsScreenViewModel)(viewModel)));
        }
        
        public virtual SettingsScreenViewModel CreateSettingsScreen() {
            return ((SettingsScreenViewModel)(this.Create(Guid.NewGuid().ToString())));
        }
        
        public override uFrame.MVVM.ViewModel CreateEmpty() {
            return new SettingsScreenViewModel(this.EventAggregator);
        }
        
        public virtual void InitializeSettingsScreen(SettingsScreenViewModel viewModel) {
            // This is called when a SettingsScreenViewModel is created
            viewModel.Apply.Action = this.ApplyHandler;
            viewModel.Default.Action = this.DefaultHandler;
            SettingsScreenViewModelManager.Add(viewModel);
        }
        
        public override void DisposingViewModel(uFrame.MVVM.ViewModel viewModel) {
            base.DisposingViewModel(viewModel);
            SettingsScreenViewModelManager.Remove(viewModel);
        }
        
        public virtual void Apply(SettingsScreenViewModel viewModel) {
        }
        
        public virtual void Default(SettingsScreenViewModel viewModel) {
        }
        
        public virtual void ApplyHandler(ApplyCommand command) {
            this.Apply(command.Sender as SettingsScreenViewModel);
        }
        
        public virtual void DefaultHandler(DefaultCommand command) {
            this.Default(command.Sender as SettingsScreenViewModel);
        }
    }
    
    public class LevelSelectScreenControllerBase : SubScreenController {
        
        private uFrame.MVVM.IViewModelManager _LevelSelectScreenViewModelManager;
        
        [uFrame.IOC.InjectAttribute("LevelSelectScreen")]
        public uFrame.MVVM.IViewModelManager LevelSelectScreenViewModelManager {
            get {
                return _LevelSelectScreenViewModelManager;
            }
            set {
                _LevelSelectScreenViewModelManager = value;
            }
        }
        
        public IEnumerable<LevelSelectScreenViewModel> LevelSelectScreenViewModels {
            get {
                return LevelSelectScreenViewModelManager.OfType<LevelSelectScreenViewModel>();
            }
        }
        
        public override void Setup() {
            base.Setup();
            // This is called when the controller is created
        }
        
        public override void Initialize(uFrame.MVVM.ViewModel viewModel) {
            base.Initialize(viewModel);
            // This is called when a viewmodel is created
            this.InitializeLevelSelectScreen(((LevelSelectScreenViewModel)(viewModel)));
        }
        
        public virtual LevelSelectScreenViewModel CreateLevelSelectScreen() {
            return ((LevelSelectScreenViewModel)(this.Create(Guid.NewGuid().ToString())));
        }
        
        public override uFrame.MVVM.ViewModel CreateEmpty() {
            return new LevelSelectScreenViewModel(this.EventAggregator);
        }
        
        public virtual void InitializeLevelSelectScreen(LevelSelectScreenViewModel viewModel) {
            // This is called when a LevelSelectScreenViewModel is created
            viewModel.SelectLevel.Action = this.SelectLevelHandler;
            LevelSelectScreenViewModelManager.Add(viewModel);
        }
        
        public override void DisposingViewModel(uFrame.MVVM.ViewModel viewModel) {
            base.DisposingViewModel(viewModel);
            LevelSelectScreenViewModelManager.Remove(viewModel);
        }
        
        public virtual void SelectLevelHandler(SelectLevelCommand command) {
            this.SelectLevel(command.Sender as LevelSelectScreenViewModel, command.Argument);
        }
        
        public virtual void SelectLevel(LevelSelectScreenViewModel viewModel, LevelDescriptor arg) {
        }
    }
    
    public class MenuScreenControllerBase : SubScreenController {
        
        private uFrame.MVVM.IViewModelManager _MenuScreenViewModelManager;
        
        [uFrame.IOC.InjectAttribute("MenuScreen")]
        public uFrame.MVVM.IViewModelManager MenuScreenViewModelManager {
            get {
                return _MenuScreenViewModelManager;
            }
            set {
                _MenuScreenViewModelManager = value;
            }
        }
        
        public IEnumerable<MenuScreenViewModel> MenuScreenViewModels {
            get {
                return MenuScreenViewModelManager.OfType<MenuScreenViewModel>();
            }
        }
        
        public override void Setup() {
            base.Setup();
            // This is called when the controller is created
        }
        
        public override void Initialize(uFrame.MVVM.ViewModel viewModel) {
            base.Initialize(viewModel);
            // This is called when a viewmodel is created
            this.InitializeMenuScreen(((MenuScreenViewModel)(viewModel)));
        }
        
        public virtual MenuScreenViewModel CreateMenuScreen() {
            return ((MenuScreenViewModel)(this.Create(Guid.NewGuid().ToString())));
        }
        
        public override uFrame.MVVM.ViewModel CreateEmpty() {
            return new MenuScreenViewModel(this.EventAggregator);
        }
        
        public virtual void InitializeMenuScreen(MenuScreenViewModel viewModel) {
            // This is called when a MenuScreenViewModel is created
            MenuScreenViewModelManager.Add(viewModel);
        }
        
        public override void DisposingViewModel(uFrame.MVVM.ViewModel viewModel) {
            base.DisposingViewModel(viewModel);
            MenuScreenViewModelManager.Remove(viewModel);
        }
    }
}
