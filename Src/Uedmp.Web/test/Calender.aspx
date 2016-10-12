<%@ Page Title="" Language="C#" MasterPageFile="~/test/test.Master" AutoEventWireup="true" CodeBehind="Calender.aspx.cs" Inherits="Uedmp.Web.test.Calender" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
<asp:CalendarExtender ID="TextBox1_CalendarExtender" runat="server" 
    Enabled="True" Format="yyyy-M-d" PopupButtonID="ImageButton1" 
    TargetControlID="TextBox1">
</asp:CalendarExtender>
<asp:ImageButton ID="ImageButton1" runat="server" 
    ImageUrl="~/images/Calendar_scheduleHS.png" />
</asp:Content>
