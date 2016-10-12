<%@ Page Title="项目管理 - 管理综合系统" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProjectManage.aspx.cs" Inherits="Uedmp.Web.Auth.Project.ProjectManage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="addNewItem">
        <asp:HyperLink ID="AddProjectHyperLink" runat="server" 
            NavigateUrl="~/Auth/Project/DemandEdit.aspx">添加项目</asp:HyperLink>
    </div>
    <asp:GridView ID="ProjectGridView" runat="server" AllowPaging="True" 
        AutoGenerateColumns="False" DataSourceID="ProjectObjectDataSource" 
        Width="80%" CssClass="box_TB" GridLines="None">
        <AlternatingRowStyle CssClass="box_td_bg_01" />
        <Columns>
            <asp:BoundField DataField="ProjectName" HeaderText="项目名称" 
                SortExpression="ProjectName" >
            <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="ProjectStatus" HeaderText="状态" 
                SortExpression="ProjectStatus" />
            <asp:BoundField DataField="PartyContacts" HeaderText="甲方联系人" 
                SortExpression="PartyContacts" />
            <asp:BoundField DataField="PartyName" HeaderText="甲方姓名" 
                SortExpression="PartyName" />
            <asp:BoundField DataField="PartyPhone" HeaderText="甲方电话" 
                SortExpression="PartyPhone" />
            <asp:TemplateField HeaderText="操作">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink4" runat="server" 
                        NavigateUrl='<%# Eval("ProjectId", "~/Auth/Project/DemandManage.aspx?ProjectID={0}") %>'>查看需求</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerSettings Mode="NumericFirstLast" />
    </asp:GridView>
    <asp:ObjectDataSource ID="ProjectObjectDataSource" runat="server" 
        OldValuesParameterFormatString="original_{0}" SelectMethod="GetAllProjects" 
        TypeName="Uedmp.Business.ProjectUtility"></asp:ObjectDataSource>
</asp:Content>
