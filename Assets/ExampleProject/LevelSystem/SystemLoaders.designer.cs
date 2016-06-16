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
    using uFrame.IOC;
    using uFrame.Kernel;
    using uFrame.MVVM;
    
    
    public class LevelSystemLoaderBase : uFrame.Kernel.SystemLoader {
        
        private LevelRootController _LevelRootController;
        
        [uFrame.IOC.InjectAttribute()]
        public virtual LevelRootController LevelRootController {
            get {
                if (_LevelRootController==null) {
                    _LevelRootController = Container.CreateInstance(typeof(LevelRootController)) as LevelRootController;;
                }
                return _LevelRootController;
            }
            set {
                _LevelRootController = value;
            }
        }
        
        public override void Load() {
            Container.RegisterViewModelManager<LevelRootViewModel>(new ViewModelManager<LevelRootViewModel>());
            Container.RegisterController<LevelRootController>(LevelRootController);
        }
    }
}
