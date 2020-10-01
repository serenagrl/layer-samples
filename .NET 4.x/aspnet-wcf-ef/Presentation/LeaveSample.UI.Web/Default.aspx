<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LeaveSample.UI.Web.Default" ViewStateMode="Disabled" EnableViewState="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  <script src="Scripts/jquery-3.5.1.min.js"></script>
  <script src="Scripts/jquery-ui-1.12.1.min.js"></script>
  <link href="Content/themes/base/jquery-ui.css" rel="stylesheet" />
  <script>
    $(function () {
        attachPicker('input[type=text][id*=startDateBox]');
        attachPicker('input[type=text][id*=endDateBox]');
        attachSpinner('input[type=text][id*=durationBox]');

        $("#accordion").accordion({
            collapsible: true
        });
    });

    function attachPicker(controlId) {
      $(controlId).datepicker(
      {
        defaultDate: null,
        minDate: -7,
        maxDate: '6M',
        beforeShowDay: $.datepicker.noWeekends,
        onSelect: showDays,
        dateFormat: "dd/mm/yy"
      });
    }

    function attachSpinner(controlId) {
      $(controlId).spinner({ min: 1, max: 90 });
    }

    function showDays(date, sender) {
      var start = $('input[type=text][id*=startDateBox]').datepicker('getDate');
      var end = $('input[type=text][id*=endDateBox]').datepicker('getDate');

      if (!start || !end)
        return;

      var days = (new Date(end - start) / 1000 / 60 / 60 / 24) + 1;

      if (days < 0) days = 0;

      $('input[type=text][id*=durationBox]').val(days);
    }
  </script>
  <div>
    <fieldset>
      <legend class="header">New Leave Details</legend>
      <asp:ValidationSummary ID="ValidationSummary1" runat="server"
        ValidationGroup="FormFields" ForeColor="Red" />
      <asp:FormView ID="leaveForm" runat="server" DefaultMode="Insert"
        ItemType="LeaveSample.Entities.Leave" OnDataBound="leaveForm_DataBound"
        InsertMethod="leaveForm_InsertItem">
        <InsertItemTemplate>
          <table style="width:100%;">
            <tr>
              <td style="width:75px"><strong>Employee</strong></td>
              <td><strong>:</strong></td>
              <td>
                <asp:Label ID="employeeLabel" runat="server" Text="<%# BindItem.Employee %>"></asp:Label>
              </td>
              <td style="width: 170px"></td>
              <td><strong>Date</strong></td>
              <td><strong>:</strong></td>
              <td>
                <asp:TextBox CssClass="textBox" ID="startDateBox" runat="server" Text="<%# BindItem.StartDate %>"></asp:TextBox>
                &nbsp;<strong>to</strong>&nbsp;
                            <asp:TextBox CssClass="textBox" ID="endDateBox" runat="server" Text="<%# BindItem.EndDate %>"></asp:TextBox>
              </td>
            </tr>
            <tr>
              <td><strong>Category</strong></td>
              <td><strong>:</strong></td>
              <td>
                <asp:RadioButtonList ID="categoryRadio" runat="server" SelectedIndex="<%# BindItem.Category %>"
                  SelectMethod="GetCategories" RepeatDirection="Horizontal" RepeatLayout="Flow">
                </asp:RadioButtonList>
              </td>
              <td></td>
              <td><strong>Duration</strong></td>
              <td><strong>:</strong></td>
              <td>
                <asp:TextBox CssClass="spinner" ID="durationBox" runat="server"
                  Text="<%# BindItem.Duration %>"></asp:TextBox>
              </td>
            </tr>
            <tr>
              <td class="topAlignedLabel"><strong>Description</strong></td>
              <td class="topAlignedLabel"><strong>:</strong></td>
              <td colspan="5">
                <asp:TextBox CssClass="multilineTextBox" ID="descriptionBox" runat="server" Text="<%# BindItem.Description %>" TextMode="MultiLine"></asp:TextBox>
              </td>
            </tr>
            <tr>
              <td>&nbsp;</td>
              <td>&nbsp;</td>
              <td colspan="5">
                <asp:LinkButton ID="applyButton" Text="Apply"
                  CommandName="Insert" runat="server" /></td>
            </tr>
          </table>
        </InsertItemTemplate>

      </asp:FormView>
    </fieldset>
    <fieldset>
      <legend class="header">Leave Records</legend>
      <table>
        <tr>
          <td style="width:75px"><strong>Category</strong></td>
          <td><strong>:</strong></td>
          <td style="width:100px">
            <asp:DropDownList ID="category" runat="server" SelectMethod="category_GetData"
              DataTextField="Text" DataValueField="Value" Width="100%" >
            </asp:DropDownList>
          </td>
          <td style="width:40px"></td>
          <td style="width:75px"><strong>Status</strong></td>
          <td><strong>:</strong></td>
          <td style="width:100px">
            <asp:DropDownList ID="status" runat="server" SelectMethod="status_GetData"
              DataTextField="Text" DataValueField="Value" Width="100%">
            </asp:DropDownList>
          </td>
          <td style="width: 20px"></td>
          <td>
            <input type="submit" value="Refresh" />
          </td>
        </tr>
      </table>
      &nbsp;<asp:GridView ID="leaveGrid" runat="server" CssClass="grid"
        ItemType="LeaveSample.Entities.Leave"
        DataKeyNames="LeaveID" SelectMethod="leaveGrid_GetData" UpdateMethod="leaveGrid_UpdateItem"
        EmptyDataText="No leave records available." PageSize="10" AllowPaging="true" AllowSorting="true"
        AutoGenerateColumns="False" Width="100%"
        AlternatingRowStyle-CssClass="grid-alt-row" PagerStyle-CssClass="grid-pager">
        <Columns>
          <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
              <asp:LinkButton ID="cancelButton" runat="server" CommandName="Update" Text="Cancel"
                Visible="<%# Item.Status == LeaveSample.Entities.LeaveStatuses.Pending %>"></asp:LinkButton>
            </ItemTemplate>
          </asp:TemplateField>
          <asp:BoundField DataField="LeaveID" HeaderText="ID" Visible="false" ItemStyle-HorizontalAlign="Center" />
          <asp:BoundField DataField="Category" HeaderText="Category" SortExpression="Category" ItemStyle-HorizontalAlign="Center" />
          <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" DataFormatString="{0:dd-MMM-yyyy}" ItemStyle-HorizontalAlign="Center" />
          <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" DataFormatString="{0:dd-MMM-yyyy}" ItemStyle-HorizontalAlign="Center" />
          <asp:BoundField DataField="Duration" HeaderText="Duration" SortExpression="Duration" ItemStyle-HorizontalAlign="Center" />
          <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ItemStyle-HorizontalAlign="Center" />
          <asp:BoundField DataField="DateSubmitted" HeaderText="Date Submitted" SortExpression="DateSubmitted" ItemStyle-HorizontalAlign="Center" />
          <asp:BoundField DataField="Description" HeaderText="Description" />
          <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
        </Columns>
      </asp:GridView>
    </fieldset>

  </div>
</asp:Content>
