<%@ Page Title="需求管理 - 管理综合系统" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DemandManage.aspx.cs" Inherits="Uedmp.Web.Auth.Project.DemandManage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="addNewItem">
        <asp:HyperLink
            ID="AddDemandHyperLink" runat="server" 
            NavigateUrl="~/Auth/Project/DemandEdit.aspx" >添加需求</asp:HyperLink>
    </div>
    <asp:GridView ID="DemandGridView" runat="server" AutoGenerateColumns="False" 
        DataSourceID="DemandObjectDataSource" Width="80%" CssClass="box_TB" 
        GridLines="None">
        <AlternatingRowStyle CssClass="box_td_bg_01" />
        <Columns>
            <asp:BoundField DataField="DemandTitle" HeaderText="标题" ReadOnly="True" 
                SortExpression="DemandTitle" >
            <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="DemandSide" HeaderText="需求方" ReadOnly="True" 
                SortExpression="DemandSide" >
            <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="CreatorDisplayName" HeaderText="提交者" ReadOnly="True" 
                SortExpression="CreatorDisplayName">
            <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="PriorityDisplayName" HeaderText="优先级" 
                ReadOnly="True" SortExpression="PriorityDisplayName">
            <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="ImportanceDisplayName" HeaderText="重要性" 
                ReadOnly="True" SortExpression="ImportanceDisplayName">
            <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="操作">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink4" runat="server" 
                        NavigateUrl='<%# Eval("DemandId", "~/Auth/Task/TaskManage.aspx?DemandID={0}") %>'>查看任务</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="DemandObjectDataSource" runat="server" 
        OldValuesParameterFormatString="original_{0}" 
        SelectMethod="GetAllDemandsByProjectId" TypeName="Uedmp.Business.DemandUtility">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="0" Name="projectId" 
                QueryStringField="ProjectID" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
