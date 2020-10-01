<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Approvals.aspx.cs" Inherits="LeaveSample.UI.Web.Approvals" ViewStateMode="Disabled" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  <div>
    <fieldset>
      <legend class="header">Approve Leaves</legend>
      <table>
        <tr>
          <td><strong>Category</strong></td>
          <td><strong>:</strong></td>
          <td>
            <asp:DropDownList ID="category" runat="server" SelectMethod="category_GetData"
              DataTextField="Text" DataValueField="Value" Width="100%">
            </asp:DropDownList>
          </td>
          <td></td>
          <td><strong>Status</strong></td>
          <td><strong>:</strong></td>
          <td>
            <asp:DropDownList ID="status" runat="server" SelectMethod="status_GetData"
              DataTextField="Text" DataValueField="Value" OnDataBound="status_DataBound" Width="100%">
            </asp:DropDownList>
          </td>
          <td><strong></strong></td>
          <td>
            <input type="submit" value="Refresh" />
          </td>
        </tr>
      </table>
      &nbsp;
        <asp:GridView ID="leaveGrid" runat="server" CssClass="grid"
          ItemType="LeaveSample.Entities.Leave"
          DataKeyNames="LeaveID" SelectMethod="leaveGrid_GetData" UpdateMethod="leaveGrid_UpdateItem"
          EmptyDataText="No leave records available." OnRowCommand="leaveGrid_RowCommand" PageSize="10"
          AllowPaging="true" AllowSorting="true"
          AutoGenerateColumns="False" Width="100%"
          AlternatingRowStyle-CssClass="grid-alt-row" PagerStyle-CssClass="grid-pager">
          <Columns>
            <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                <asp:LinkButton ID="editButton" runat="server" CommandName="Edit" Text="Select"
                  Visible="<%# Item.Status == LeaveSample.Entities.LeaveStatuses.Pending %>"></asp:LinkButton>
              </ItemTemplate>
              <EditItemTemplate>
                <asp:LinkButton ID="approveButton" runat="server" CommandName="Update" CommandArgument="Approve" Text="Approve"></asp:LinkButton> 
                | <asp:LinkButton ID="rejectButton" runat="server" CommandName="Update" CommandArgument="Reject" Text="Reject"></asp:LinkButton> 
                | <asp:LinkButton ID="cancelButton" runat="server" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
              </EditItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="LeaveID" HeaderText="ID" Visible="false" />
            <asp:TemplateField HeaderText="Employee" SortExpression="Employee" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# Item.Employee %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Category" SortExpression="Category" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                <asp:Label ID="Label2" runat="server" Text='<%# Item.Category %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                <asp:Label ID="Label3" runat="server" Text='<%# Eval("StartDate", "{0:dd-MMM-yyyy}") %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End Date" SortExpression="EndDate" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                <asp:Label ID="Label4" runat="server" Text='<%# Eval("EndDate", "{0:dd-MMM-yyyy}") %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Duration" SortExpression="Duration" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                <asp:Label ID="Label5" runat="server" Text='<%# Item.Duration %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status" SortExpression="Status" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                <asp:Label ID="Label6" runat="server" Text='<%# Item.Status %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Date Submitted" SortExpression="DateSubmitted" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                <asp:Label ID="Label7" runat="server" Text='<%# Item.DateSubmitted %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Description">
              <ItemTemplate>
                <asp:Label ID="Label8" runat="server" Text='<%# Item.Description %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Remarks">
              <ItemTemplate>
                <asp:Label ID="Label9" runat="server" Text='<%# Item.Remarks %>'></asp:Label>
              </ItemTemplate>
              <EditItemTemplate>
                <asp:TextBox CssClass="multilineTextBox" ID="remarksBox" runat="server" Width="98%"
                  Text="<%# BindItem.Remarks %>" TextMode="MultiLine"></asp:TextBox>
              </EditItemTemplate>
            </asp:TemplateField>
          </Columns>
        </asp:GridView>
    </fieldset>
  </div>
</asp:Content>

