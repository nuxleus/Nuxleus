using System;
using System.Collections.Generic;
using System.Text;

namespace Enyim.Reflection
{
	/// <summary>
	/// This property supports the <see cref="T:FastActivator"/> and is not intended to be used directly from your code.
	/// </summary>
	public interface IFastObjectFacory
	{
		object CreateInstance();
	}
}

#region [ License information          ]
/* ************************************************************
 *
 * Copyright (c) Attila Kisk�, enyim.com, 2007
 *
 * This source code is subject to terms and conditions of 
 * Microsoft Permissive License (Ms-PL).
 * 
 * A copy of the license can be found in the License.html
 * file at the root of this distribution. If you can not 
 * locate the License, please send an email to a@enyim.com
 * 
 * By using this source code in any fashion, you are 
 * agreeing to be bound by the terms of the Microsoft 
 * Permissive License.
 *
 * You must not remove this notice, or any other, from this
 * software.
 *
 * ************************************************************/
#endregion