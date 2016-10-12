<%@ Page Title="编辑需求 - 管理综合系统" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DemandEdit.aspx.cs" Inherits="Uedmp.Web.Auth.Project.DemandEdit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            height: 22px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <fieldset>
        <legend>
            <asp:Label ID="EditDemandLabel" runat="server" Text="编辑需求" /></legend>
        <table class="style1" border="0" cellpadding="5" cellspacing="5">
            <tr>
                <td class="style2">
                    <asp:Label ID="Label2" runat="server" Text="*" CssClass="requiredTip"></asp:Label>
                    <asp:Label ID="Label1" runat="server" Text="需求方" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:TextBox ID="DemandSideTextBox" runat="server" CssClass="inputEntry " MaxLength="100"
                        Width="240px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="DemandSideTextBox" CssClass="validator" Display="Dynamic" 
                        ErrorMessage="请填写需求方"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator1_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator1">
                    </asp:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label4" runat="server" Text="标题" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:TextBox ID="TitleTextBox" runat="server" CssClass="inputEntry" 
                        MaxLength="100" Width="240px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        CssClass="validator" Display="Dynamic" ErrorMessage="请填写标题" 
                        ControlToValidate="TitleTextBox"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator2_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator2">
                    </asp:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label6" runat="server" Text="内容" CssClass="inputLabel"></asp:Label>
                    <br />
                    <div>
                        <asp:TextBox ID="ContentTextBox" runat="server" CssClass="inputEntry" Rows="15" TextMode="MultiLine"
                            Width="80%"></asp:TextBox>
                        <asp:HtmlEditorExtender ID="HtmlEditorExtender1" runat="server" 
                            TargetControlID="ContentTextBox">
                        </asp:HtmlEditorExtender>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                            CssClass="validator" Display="Dynamic" ErrorMessage="请填写内容" 
                            ControlToValidate="ContentTextBox"></asp:RequiredFieldValidator>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label8" runat="server" Text="优先级" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:DropDownList ID="DemandPriorityDropDownList" runat="server" 
                        CssClass="inputEntry" Width="150px" 
                        DataSourceID="DemandPriorityObjectDataSource" 
                        DataTextField="DemandPriorityDisplayName" 
                        DataValueField="DemandPriorityID" AppendDataBoundItems="True">
                        <asp:ListItem Value="0">请选择</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                        ControlToValidate="DemandPriorityDropDownList" CssClass="validator" 
                        Display="Dynamic" ErrorMessage="请选择优先级" InitialValue="0"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator4_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator4">
                    </asp:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label9" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label10" runat="server" Text="重要性" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:DropDownList ID="DemandImportanceDropDownList" runat="server" 
                        CssClass="inputEntry" Width="150px" AppendDataBoundItems="True" 
                        DataSourceID="DemandImportanceObjectDataSource" 
                        DataTextField="DemandImportanceDisplayName" DataValueField="DemandImportanceID">
                        <asp:ListItem Value="0">请选择</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                        ControlToValidate="DemandImportanceDropDownList" CssClass="validator" 
                        Display="Dynamic" ErrorMessage="请选择重要性" InitialValue="0"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator5_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator5">
                    </asp:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button ID="SaveButton" runat="server" CssClass="saveButton" Text="保 存" 
                        onclick="SaveButton_Click" />
                    &nbsp;
                    <asp:LinkButton ID="CancelLinkButton" runat="server" CssClass="cancelButton" 
                        CausesValidation="False" onclick="CancelLinkButton_Click">取消</asp:LinkButton>
                    <asp:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server" ConfirmText="您确认要取消吗？"
                        TargetControlID="CancelLinkButton">
                    </asp:ConfirmButtonExtender>
                </td>
            </tr>
        </table>
    </fieldset>
    <asp:ObjectDataSource ID="DemandPriorityObjectDataSource" runat="server" 
        OldValuesParameterFormatString="original_{0}" 
        SelectMethod="GetAllDemandPriorities" TypeName="Uedmp.Business.CommonUtility"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="DemandImportanceObjectDataSource" runat="server" 
        OldValuesParameterFormatString="original_{0}" 
        SelectMethod="GetAllDemandImportances" TypeName="Uedmp.Business.CommonUtility"></asp:ObjectDataSource>
</asp:Content>
