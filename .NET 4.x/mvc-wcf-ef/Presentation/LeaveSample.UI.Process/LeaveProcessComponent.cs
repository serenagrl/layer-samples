//====================================================================================================
// Base code generated with Impulse: UIPC Gen (Build 2.1.4877.28464)
// Layered Architecture Solution Guidance (http://layerguidance.codeplex.com)
//
// Generated by Serena Yeoh at ALIENWARE on 05/09/2013 16:53:21 
//====================================================================================================

using LeaveSample.Entities;
using LeaveSample.Services.Contracts;
using LeaveSample.UI.Process.LeaveService;
using LeaveSample.UI.Process.LeaveWorkflowService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

// NOTE:
//
// User Process Components are used to abstract the common processing task from 
// the UI or control the UI navigation logic. You can also treat UPCs as 
// Controllers (the Entities as the Model and the UI Forms as Views).
//
// This sample is too simplistic to show any navigation logic. However, it does
// illustrate how to abstract the service communication logic away from the UI.

namespace LeaveSample.UI.Process
{
    /// <summary>
    /// Leave controller component.
    /// </summary>
    public partial class LeaveProcessComponent
    {
        /// <summary>
        /// Calls the ListLogsByLeave operation method in the LeaveService.
        /// </summary>
        /// <param name="leaveID">A leaveID value.</param>
        /// <returns>Returns a List<LeaveStatusLog> object.</returns>
        public List<LeaveStatusLog> ListLogsByLeave(long leaveID)
        {
            List<LeaveStatusLog> result = default(List<LeaveStatusLog>);
            LeaveServiceClient proxy = new LeaveServiceClient();

            try
            {
                result = proxy.ListLogsByLeave(leaveID);
            }
            catch (FaultException fex)
            {
                // TODO: Handle your exception here or raise it to the UI.
                //		 Do not display sensitive information to the UI.
                throw new ApplicationException(fex.Message);
            }
            finally
            {
                proxy.Close();
            }
            return result;
        }

        /// <summary>
        /// Calls the Reject operation method in the LeaveWorkflowService.
        /// </summary>
        /// <param name="leave">A leave value.</param>
        public Leave Reject(Leave leave)
        {
            LeaveWorkflowServiceClient proxy = new LeaveWorkflowServiceClient();

            try
            {
                return proxy.Reject(leave);
            }
            catch (FaultException fex)
            {
                // TODO: Handle your exception here or raise it to the UI.
                //		 Do not display sensitive information to the UI.
                throw new ApplicationException(fex.Message);
            }
            finally
            {
                proxy.Close();
            }
        }

        /// <summary>
        /// Calls the Approve operation method in the LeaveWorkflowService.
        /// </summary>
        /// <param name="leave">A leave value.</param>
        public Leave Approve(Leave leave)
        {
            LeaveWorkflowServiceClient proxy = new LeaveWorkflowServiceClient();

            try
            {
                return proxy.Approve(leave);
            }
            catch (FaultException fex)
            {
                // TODO: Handle your exception here or raise it to the UI.
                //		 Do not display sensitive information to the UI.
                throw new ApplicationException(fex.Message);
            }
            finally
            {
                proxy.Close();
            }
        }

        /// <summary>
        /// Calls the Cancel operation method in the LeaveWorkflowService.
        /// </summary>
        /// <param name="leave">A leave value.</param>
        public Leave Cancel(Leave leave)
        {
            LeaveWorkflowServiceClient proxy = new LeaveWorkflowServiceClient();

            try
            {
                return proxy.Cancel(leave);
            }
            catch (FaultException fex)
            {
                // TODO: Handle your exception here or raise it to the UI.
                //		 Do not display sensitive information to the UI.
                throw new ApplicationException(fex.Message);
            }
            finally
            {
                proxy.Close();
            }
        }

        /// <summary>
        /// Calls the Apply operation method in the LeaveWorkflowService.
        /// </summary>
        /// <param name="leave">A leave value.</param>
        public Leave Apply(Leave leave)
        {
            LeaveWorkflowServiceClient proxy = new LeaveWorkflowServiceClient();

            try
            {
                leave.CorrelationID = Guid.NewGuid();
                return proxy.Apply(leave);
            }
            catch (FaultException fex)
            {
                // TODO: Handle your exception here or raise it to the UI.
                //		 Do not display sensitive information to the UI.
                throw new ApplicationException(fex.Message);
            }
            finally
            {
                proxy.Close();
            }
        }


        /// <summary>
        /// Calls the GetLeaveById operation method in the LeaveService.
        /// </summary>
        /// <param name="leaveID">A leaveID value.</param>
        /// <returns>Returns a Leave object.</returns>
        public Leave GetLeaveById(long leaveID)
        {
            Leave result = default(Leave);
            LeaveServiceClient proxy = new LeaveServiceClient();

            try
            {
                result = proxy.GetLeaveById(leaveID);
            }
            catch (FaultException fex)
            {
                // TODO: Handle your exception here or raise it to the UI.
                //		 Do not display sensitive information to the UI.
                throw new ApplicationException(fex.Message);
            }
            finally
            {
                proxy.Close();
            }
            return result;
        }

        /// <summary>
        /// Calls the ListLeavesByEmployee operation method in the LeaveService.
        /// </summary>
        /// <param name="maximumRows">A maximumRows value.</param>
        /// <param name="startRowIndex">A startRowIndex value.</param>
        /// <param name="sortExpression">A sortExpression value.</param>
        /// <param name="employee">A employee value.</param>
        /// <param name="category">A category value.</param>
        /// <param name="status">A status value.</param>
        /// <param name="int">A int value.</param>
        /// <returns>Returns a List<Leave> object.</returns>
        public List<Leave> ListLeavesByEmployee(int maximumRows, int startRowIndex, 
            string sortExpression, string employee, LeaveCategories? category, LeaveStatuses? status, 
            out int totalRowCount)
        {
            List<Leave> result = default(List<Leave>);
            LeaveServiceClient proxy = new LeaveServiceClient();

            try
            {
                result = proxy.ListLeavesByEmployee(maximumRows, startRowIndex, sortExpression, 
                    employee, category, status, out totalRowCount);
            }
            catch (FaultException fex)
            {
                // TODO: Handle your exception here or raise it to the UI.
                //		 Do not display sensitive information to the UI.
                throw new ApplicationException(fex.Message);
            }
            finally
            {
                proxy.Close();
            }
            return result;
        }
    }
}
