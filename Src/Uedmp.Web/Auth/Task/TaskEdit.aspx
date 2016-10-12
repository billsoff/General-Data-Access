<%@ Page Title="编辑任务 - 管理综合系统" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="TaskEdit.aspx.cs" Inherits="Uedmp.Web.Auth.Task.TaskEdit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="htmlEditor" %>
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
            <asp:Label ID="EditDemandLabel" runat="server" Text="编辑任务" /></legend>
        <table class="style1" border="0" cellpadding="5" cellspacing="5">
            <tr>
                <td class="style2" width="100%">
                    <asp:Label ID="Label2" runat="server" Text="*" CssClass="requiredTip"></asp:Label>
                    <asp:Label ID="Label1" runat="server" Text="项目" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ProjectDropDownList" runat="server" CssClass="inputEntry" Width="240px"
                        AppendDataBoundItems="True" AutoPostBack="True" DataSourceID="ProjectObjectDataSource"
                        DataTextField="ProjectName" DataValueField="ProjectId">
                        <asp:ListItem Value="0">请选择</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label4" runat="server" Text="需求" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:DropDownList ID="DemandDropDownList" runat="server" CssClass="inputEntry" Width="240px"
                        AppendDataBoundItems="True" DataSourceID="DemandObjectDataSource" DataTextField="DemandTitle"
                        DataValueField="DemandId">
                        <asp:ListItem Value="0">请选择</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="DemandDropDownList" Display="Dynamic" ErrorMessage="请选择需求" 
                        InitialValue="0" CssClass="validator"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator1_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator1">
                    </asp:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label13" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label14" runat="server" Text="接受者" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:DropDownList ID="TaskWorkerDropDownList" runat="server" CssClass="inputEntry"
                        Width="80px" AppendDataBoundItems="True" DataSourceID="WorkerObjectDataSource"
                        DataTextField="Name" DataValueField="UserId">
                        <asp:ListItem Value="0">请选择</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="TaskWorkerDropDownList" Display="Dynamic" 
                        ErrorMessage="请选择任务接受者" InitialValue="0" CssClass="validator"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator2_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator2">
                    </asp:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label15" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label16" runat="server" Text="名称" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:TextBox ID="TaskNameTextBox" runat="server" CssClass="inputEntry" MaxLength="60"
                        Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                        ControlToValidate="TaskNameTextBox" Display="Dynamic" 
                        ErrorMessage="请选择任务名称" CssClass="validator"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator3_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator3">
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
                        <asp:HtmlEditorExtender ID="HtmlEditorExtender1" runat="server" TargetControlID="ContentTextBox">
                        </asp:HtmlEditorExtender>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                            Display="Dynamic" ErrorMessage="请填写任务内容" 
                            ControlToValidate="ContentTextBox" CssClass="validator"></asp:RequiredFieldValidator>
                        <asp:ValidatorCalloutExtender ID="RequiredFieldValidator4_ValidatorCalloutExtender" 
                            runat="server" Enabled="True" TargetControlID="RequiredFieldValidator4">
                        </asp:ValidatorCalloutExtender>
                    </div>
                </td>
            </tr>
            <tr>
                <td width="100%">
                    <asp:Label ID="Label7" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label17" runat="server" Text="开始时间" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:TextBox ID="StartTimeDateTextBox" runat="server" CssClass="inputEntry" Width="100px"></asp:TextBox>
                    <asp:CalendarExtender ID="StartTimeDateTextBox_CalendarExtender" runat="server" Enabled="True"
                        TargetControlID="StartTimeDateTextBox">
                    </asp:CalendarExtender>
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                        ControlToValidate="StartTimeDateTextBox" Display="Dynamic" 
                        ErrorMessage="请填写开始日期" CssClass="validator"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator5_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator5">
                    </asp:ValidatorCalloutExtender>
                    <asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ControlToValidate="StartTimeDateTextBox" Display="Dynamic" 
                        ErrorMessage="开始日期格式不正确" Operator="DataTypeCheck" Type="Date" 
                        CssClass="validator" Enabled="False"></asp:CompareValidator>
                    <asp:ValidatorCalloutExtender ID="CompareValidator1_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="CompareValidator1">
                    </asp:ValidatorCalloutExtender>
                    <asp:DropDownList ID="StartTimeHourDropDownList" runat="server" CssClass="inputEntry"
                        Width="50px" DataSourceID="HourObjectDataSource">
                    </asp:DropDownList>
                    <asp:Label ID="Label18" runat="server" Text="点"></asp:Label>
                    &nbsp;<asp:DropDownList ID="StartTimeMinuteDropDownList" runat="server" CssClass="inputEntry"
                        Width="50px" DataSourceID="MinuteObjectDataSource">
                    </asp:DropDownList>
                    <asp:Label ID="Label19" runat="server" Text="分"></asp:Label>
                </td>
            </tr>
            <tr>
                <td width="100%">
                    <asp:Label ID="Label20" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label21" runat="server" Text="结束时间" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:TextBox ID="EndTimeDateTextBox" runat="server" CssClass="inputEntry" Width="100px"></asp:TextBox>
                    <asp:CalendarExtender ID="EndTimeDateTextBox_CalendarExtender" runat="server" Enabled="True"
                        TargetControlID="EndTimeDateTextBox">
                    </asp:CalendarExtender>
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                        ControlToValidate="EndTimeDateTextBox" Display="Dynamic" 
                        ErrorMessage="请填写结束日期" CssClass="validator"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator6_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator6">
                    </asp:ValidatorCalloutExtender>
                    <asp:CompareValidator ID="CompareValidator3" runat="server" 
                        ControlToValidate="EndTimeDateTextBox" CssClass="validator" Display="Dynamic" 
                        ErrorMessage="结束日期格式不正确" Operator="DataTypeCheck" Type="Date" 
                        Enabled="False"></asp:CompareValidator>
                    <asp:ValidatorCalloutExtender ID="CompareValidator3_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="CompareValidator3">
                    </asp:ValidatorCalloutExtender>
                    <asp:DropDownList ID="EndTimeHourDropDownList" runat="server" CssClass="inputEntry"
                        Width="50px" DataSourceID="HourObjectDataSource">
                    </asp:DropDownList>
                    <asp:Label ID="Label22" runat="server" Text="点"></asp:Label>
                    &nbsp;<asp:DropDownList ID="EndTimeMinuteDropDownList" runat="server" CssClass="inputEntry"
                        Width="50px" DataSourceID="MinuteObjectDataSource">
                    </asp:DropDownList>
                    <asp:Label ID="Label23" runat="server" Text="分"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label24" runat="server" CssClass="requiredTip" Text="*"></asp:Label>
                    <asp:Label ID="Label25" runat="server" Text="工时" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:TextBox ID="WorkTimeTextBox" runat="server" CssClass="inputEntry" Width="70px"></asp:TextBox>
                    &nbsp;<asp:Label ID="Label26" runat="server" Text="小时"></asp:Label>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                        ControlToValidate="WorkTimeTextBox" CssClass="validator" Display="Dynamic" 
                        ErrorMessage="请填写工时"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="RequiredFieldValidator7_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="RequiredFieldValidator7">
                    </asp:ValidatorCalloutExtender>
                    <asp:CompareValidator ID="CompareValidator2" runat="server" 
                        ControlToValidate="WorkTimeTextBox" CssClass="validator" Display="Dynamic" 
                        ErrorMessage="工时必须为一个正实数" Operator="GreaterThan" Type="Double" 
                        ValueToCompare="0.0"></asp:CompareValidator>
                    <asp:ValidatorCalloutExtender ID="CompareValidator2_ValidatorCalloutExtender" 
                        runat="server" Enabled="True" TargetControlID="CompareValidator2">
                    </asp:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label27" runat="server" Text="*" CssClass="requiredTip"></asp:Label>
                    <asp:Label ID="Label28" runat="server" Text="状态" CssClass="inputLabel"></asp:Label>
                    <br />
                    <asp:DropDownList ID="TaskStateDropDownList" runat="server" CssClass="inputEntry"
                        Width="70px" AppendDataBoundItems="True" DataSourceID="TaskStateObjectDataSource"
                        DataTextField="TaskStateDisplayName" DataValueField="TaskStateID">
                        <asp:ListItem Value="0">请选择</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="center">
                <htmlEditor:Editor runat="server" Content="my text" Height="300px" Width="90%" />
                    </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button ID="SaveButton" runat="server" CssClass="saveButton" Text="保 存" OnClick="SaveButton_Click" />
                    &nbsp;
                    <asp:LinkButton ID="CancelLinkButton" runat="server" CssClass="cancelButton" CausesValidation="False"
                        OnClick="CancelLinkButton_Click">取消</asp:LinkButton>
                    <asp:ConfirmButtonExtender ID="CancelLinkButton_ConfirmButtonExtender" runat="server"
                        ConfirmText="你确认要取消吗？" Enabled="True" TargetControlID="CancelLinkButton">
                    </asp:ConfirmButtonExtender>
                </td>
            </tr>
        </table>
    </fieldset>
    <asp:ObjectDataSource ID="ProjectObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetAllProjects" TypeName="Uedmp.Business.ProjectUtility"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="DemandObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetAllDemandsByProjectId" TypeName="Uedmp.Business.DemandUtility">
        <SelectParameters>
            <asp:ControlParameter ControlID="ProjectDropDownList" Name="projectId" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="WorkerObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetAllUsers" TypeName="Uedmp.Business.UserUtility"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="HourObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetHours" TypeName="Uedmp.Business.CommonUtility"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="MinuteObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetMinutes" TypeName="Uedmp.Business.CommonUtility"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="TaskStateObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetAllTaskStates" TypeName="Uedmp.Business.CommonUtility"></asp:ObjectDataSource>
</asp:Content>
