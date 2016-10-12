<%@ Page Title="任务管理 - 管理综合系统" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="TaskManage.aspx.cs" Inherits="Uedmp.Web.Auth.Task.TaskManage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="addNewItem">
        <asp:HyperLink ID="HyperLink1" runat="server" 
            NavigateUrl="~/Auth/Task/TaskEdit.aspx">添加任务</asp:HyperLink>
    </div>
    <asp:GridView ID="TaskGridView" runat="server" AutoGenerateColumns="False" 
        DataSourceID="TaskObjectDataSource" Width="80%" CssClass="box_TB" 
        GridLines="None">
        <AlternatingRowStyle CssClass="box_td_bg_01" />
        <Columns>
            <asp:BoundField DataField="TaskName" HeaderText="任务名称" 
                SortExpression="TaskName" />
            <asp:BoundField DataField="AssignerName" HeaderText="任务分配者" 
                SortExpression="Demand">
            <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="WorkerName" HeaderText="任务接受者" 
                SortExpression="Demand">
            <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="StateDisplayName" HeaderText="状态" 
                SortExpression="Demand">
            <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="StartTime" DataFormatString="{0:yyyy-MM-dd HH:mm}" 
                HeaderText="开始时间" SortExpression="StartTime">
            <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="EndTime" DataFormatString="{0:yyyy-MM-dd HH:mm}" 
                HeaderText="结束时间" SortExpression="EndTime">
            <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="WorkTime" DataFormatString="{0:#,##0.0}" 
                HeaderText="工时" SortExpression="WorkTime">
            <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="TaskObjectDataSource" runat="server" 
        OldValuesParameterFormatString="original_{0}" SelectMethod="GetAllMyTasks" 
        TypeName="Uedmp.Business.TaskUtility"></asp:ObjectDataSource>
</asp:Content>
