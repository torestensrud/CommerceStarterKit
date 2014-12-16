<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigurePayment.ascx.cs" Inherits="OxxCommerceStarterKit.Core.PaymentProviders.DIBS.ConfigurePayment" %>
<div id="DataForm">
    <table cellpadding="0" cellspacing="2">
        <tr>
            <td class="FormLabelCell" colspan="2"><b>
                <asp:Literal ID="Literal1" runat="server" Text="Configure DIBS Account" /></b></td>
        </tr>
    </table>
    <br />
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:OrderStrings, Payment_Merchant_ID %>" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="User" Width="230"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="User" Display="dynamic" Font-Name="verdana" Font-Size="9pt" ErrorMessage="<%$ Resources:OrderStrings, Payment_User_Required %>"
                    runat="server" ID="RequiredFieldValidator2"></asp:RequiredFieldValidator>
            </td>
        </tr>      
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:SharedStrings, Processing_URL %>" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="ProcessingUrl" Width="300px"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="ProcessingUrl" Display="dynamic" Font-Name="verdana" Font-Size="9pt"
                    ErrorMessage="<%$ Resources:OrderStrings, Payment_Processing_Url_Required %>" runat="server" ID="Requiredfieldvalidator5"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>

        <tr>
            <td colspan="2" class="FormSpacerCell">Dibs Payment Window</td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Literal ID="Literal6" runat="server" Text="HMAC key for MAC calculation" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="Key" Width="300px"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="Key" Display="dynamic" Font-Name="verdana" Font-Size="9pt"
                    ErrorMessage="Key is required" runat="server" ID="Requiredfieldvalidator6"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</div>
