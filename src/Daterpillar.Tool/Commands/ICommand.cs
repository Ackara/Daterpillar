using System;
using System.Collections.Generic;
using System.Linq;

namespace Acklann.Daterpillar.Commands
{
	public interface ICommand
	{
		int Execute();
	}
}