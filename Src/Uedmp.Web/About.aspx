<%@ Page Title="关于我们" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="Uedmp.Web.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        关于
    </h2>
    <p>
        将内容放置在此处。
    </p>
    <p>
        <asp:HyperLink ID="HyperLink4" runat="server" 
            NavigateUrl="~/Auth/Task/TaskEdit.aspx">HyperLink</asp:HyperLink>
    </p>
</asp:Content>
