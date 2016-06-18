using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using uFrame.Kernel;
using uFrame.IOC;
using uFrame.MVVM;
using uFrame.Serialization;

namespace uFrame.ExampleProject
{
	public class LevelRootController : LevelRootControllerBase
	{

		public override void InitializeLevelRoot (LevelRootViewModel viewModel)
		{
			base.InitializeLevelRoot (viewModel);
		}

		public override void FinishCurrentLevel (LevelRootViewModel viewModel)
		{
			base.FinishCurrentLevel (viewModel);

			viewModel.StateProperty.Level_Close.OnNext (true);
		}
	}
}

