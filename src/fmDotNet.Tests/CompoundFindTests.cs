﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace fmDotNet.Tests
{
    [TestClass]
    public class CompoundFindTests
    {
        public CompoundFindTests() { }

        FMSAxml SetupFMSAxml()
        {
            var asr = new System.Configuration.AppSettingsReader();

            var fms = new FMSAxml(
                theServer: (string)asr.GetValue("TestServerName", typeof(string)),
                theAccount: (string)asr.GetValue("TestServerUser", typeof(string)),
                thePort: (int)asr.GetValue("TestServerPort", typeof(int)),
                thePW: (string)asr.GetValue("TestServerPass", typeof(string))
                );
            return fms;
        }

        [TestMethod]
        public void CompoundFind_Red_OR_Blue_ReturnsRedPlusBlue()
        {
            // arrange 
            var fms = this.SetupFMSAxml();
            fms.SetDatabase("fmDotNet.Tests", false);
            fms.SetLayout("FindRequest.Tests");
            // find how many red and how many blue we have
            var find = fms.CreateFindRequest(Enumerations.SearchType.AllRecords);
            DataSet res = find.Execute();
            var blueCount = 0;
            var redCount = 0;
            foreach (DataRow dr in res.Tables[0].Rows)
            {
                if (dr["Colors::Name"].ToString() == "Red") { redCount++; }
                if (dr["Colors::Name"].ToString() == "Blue") { blueCount++; }
            }

            // act
            var cpfRequest = fms.CreateCompoundFindRequest();
            cpfRequest.AddSearchCriterion("Colors::Name", "Blue", true, false);
            cpfRequest.AddSearchCriterion("Colors::Name", "Red", true, false);
            var response = cpfRequest.Execute();

            // assert
            Assert.IsTrue(blueCount >= 1, "Must have one or more Blues.");
            Assert.IsTrue(redCount >= 1, "Must have one or more Reds.");
            Assert.AreEqual(blueCount+redCount, response.Tables[0].Rows.Count);
        }
    }
}