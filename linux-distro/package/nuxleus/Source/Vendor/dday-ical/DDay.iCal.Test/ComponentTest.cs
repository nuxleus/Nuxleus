using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DDay.iCal.Components;

namespace DDay.iCal.Test
{
    [TestFixture]
    public class ComponentTest
    {
        [Test, Category("Component")]
        public void UniqueComponent1()
        {
            iCalendar iCal = new iCalendar();
            Event evt = iCal.Create<Event>();

            Assert.IsNotNull(evt.UID);
            Assert.IsNotNull(evt.Created);
            Assert.IsNotNull(evt.DTStamp);
        }
    }
}
