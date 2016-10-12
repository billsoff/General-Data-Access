<%@ Page Title="" Language="C#" MasterPageFile="~/test/test.Master" AutoEventWireup="true" CodeBehind="HtmlEditorExtender.aspx.cs" Inherits="Uedmp.Web.test.HtmlEditorExtender" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:TextBox ID="TextBox1" runat="server" Rows="15" TextMode="MultiLine" 
        Width="300px"></asp:TextBox>
    <asp:HtmlEditorExtender ID="TextBox1_HtmlEditorExtender" runat="server" 
        Enabled="True" TargetControlID="TextBox1">
    </asp:HtmlEditorExtender>
</asp:Content>
