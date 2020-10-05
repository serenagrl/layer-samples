using LeaveSample.UI.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LeaveSample.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace LeaveSample.Test
{
    /// <summary>
    ///This is a test class for LeaveControllerTest and is intended
    ///to contain all LeaveControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LeaveControllerTest
    {

        private Leave _leave;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (_leave != null)
            {
                using(var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
                {
                    using(var cmd = new SqlCommand("DELETE FROM LeaveStatusLogs WHERE LeaveID=@LeaveID", cn))
                    {
                        cn.Open();
                        cmd.Parameters.AddWithValue("@LeaveId", _leave.LeaveID);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM Leaves WHERE LeaveID=@LeaveID";
                        cmd.ExecuteNonQuery();
                    }
                }

            }
        }

        private Leave CreateLeave(string description)
        {
            int modifier = new Random(30).Next(100);

            Leave leave = new Leave()
            {
                StartDate = DateTime.Now.AddDays(modifier),
                EndDate = DateTime.Now.AddDays(modifier),
                Duration = 1,
                Category = LeaveCategories.Annual,
                Employee = "Unit Test",
                Description = description,
            };

            return leave;
        }

        #endregion

        /// <summary>
        ///A test for Applying leaves with overlapping dates.
        ///</summary>
        [TestMethod()]
        public void DuplicateApplyTest()
        {
            _leave = CreateLeave("Unit Test: Apply with overlapping date.");

            LeaveController upc = new LeaveController();

            // 1. Apply a new leave.
            _leave = upc.Apply(_leave);

            Assert.AreEqual(_leave.Status, LeaveStatuses.Pending, "Failed to apply new leave.");

            // Keep the previous workflow instance.
            Guid correlationId = _leave.CorrelationID;

            try
            {
                // 2. This will cause exception to occur.
                upc.Apply(_leave);
            }
            catch (ApplicationException ex)
            {
                if (ex.Message != "Date range is overlapping with another leave.")
                    throw ex;
            }

            // 3. Cancel it.
            // Need to do this, otherwise, there will be an orphaned workflow instance.
            _leave.CorrelationID = correlationId;
            upc.Cancel(_leave);

            _leave = upc.GetLeaveById(_leave.LeaveID);
            Assert.AreEqual(_leave.Status, LeaveStatuses.Cancelled, "Failed to cancel leave.");
        }

        /// <summary>
        ///A test for Apply
        ///</summary>
        [TestMethod()]
        public void ApplyThenCancelTest()
        {
            _leave = CreateLeave("Unit Test: Apply Then Cancel");

            LeaveController upc = new LeaveController(); 

            // 1. Apply a new leave.
            _leave = upc.Apply(_leave);

            Assert.AreEqual(_leave.Status, LeaveStatuses.Pending, "Failed to apply new leave.");

            // 2. Cancel it.
            upc.Cancel(_leave);

            _leave = upc.GetLeaveById(_leave.LeaveID);
            Assert.AreEqual(_leave.Status, LeaveStatuses.Cancelled, "Failed to cancel leave.");
        }

      
        /// <summary>
        ///A test for Approve
        ///</summary>
        [TestMethod()]
        public void ApplyThenApproveTest()
        {
            _leave = CreateLeave("Unit Test: Apply Then Approve");

            LeaveController upc = new LeaveController();

            // 1. Apply a new leave.
            _leave = upc.Apply(_leave);

            Assert.AreEqual(_leave.Status, LeaveStatuses.Pending, "Failed to apply new leave.");

            // 2. Approve it.
            upc.Approve(_leave);

            _leave = upc.GetLeaveById(_leave.LeaveID);
            Assert.AreEqual(_leave.Status, LeaveStatuses.Approved, "Failed to approve leave.");
        }

        /// <summary>
        ///A test for Reject
        ///</summary>
        [TestMethod()]
        public void ApplyThenRejectTest()
        {
            _leave = CreateLeave("Unit Test: Apply Then Reject");

            LeaveController upc = new LeaveController();

            // 1. Apply a new leave.
            _leave = upc.Apply(_leave);

            Assert.AreEqual(_leave.Status, LeaveStatuses.Pending, "Failed to apply new leave.");

            // 2. Reject it.
            upc.Reject(_leave);

            _leave = upc.GetLeaveById(_leave.LeaveID);
            Assert.AreEqual(_leave.Status, LeaveStatuses.Rejected, "Failed to reject leave.");
        }
    }
}
