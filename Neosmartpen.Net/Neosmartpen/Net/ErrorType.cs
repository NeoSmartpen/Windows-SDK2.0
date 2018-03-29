using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosmartpen.Net
{
	public enum ErrorType
	{
		MissingPenUp = 1,
		MissingPenDown = 2,
		InvalidTime = 3,
		MissingPenDownPenMove = 4,
		FilteredCode = 5,
		NdacError = 6
	}
}
