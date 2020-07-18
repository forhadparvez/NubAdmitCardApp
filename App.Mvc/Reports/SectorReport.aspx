<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SectorReport.aspx.cs" Inherits="App.Mvc.Reports.SectorReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<form id="formCustomerReport" runat="server">  
    <div>  
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%"></rsweb:ReportViewer>
    </div>  
</form>
