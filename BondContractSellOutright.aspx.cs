using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UI_ModalPopup_BondContractSellOutright : System.Web.UI.Page
{
    //private csUtils objUtils = new csUtils();
    private csHTML objHTML = new csHTML();
    private int _PageSize = 15;
    private int _CurrentPage = 1;
    private string strIssuerCode = "";
    private string strTxtReceiver = "";
    private string strBtnCloseId = "";
    private string codeFilter = "";

    void Page_Init(object sender, EventArgs e)
    {
        Page.Form.DefaultButton = btnFilter.UniqueID;
        Page.Form.DefaultFocus = txtContractCode.ClientID;

    }

    protected void Page_Load(object sender, EventArgs e)
    {

        strIssuerCode = Session["SH_WORKING_ISSUER_CODE"].ToString();

        if (Request.QueryString["txtReceiver"] != null && Request.QueryString["txtReceiver"].ToString() != "")
            strTxtReceiver = Request.QueryString["txtReceiver"].ToString();
        if (Request.QueryString["btnClose"] != null && Request.QueryString["btnClose"].ToString() != "")
            strBtnCloseId = Request.QueryString["btnClose"].ToString();
        if (Request.QueryString["codeFilter"] != null && Request.QueryString["codeFilter"].ToString() != "")
            codeFilter = Request.QueryString["codeFilter"].ToString();

        //Bat phim escape
        this.Form.Attributes.Add("onKeyDown", "PopupWindowOnKeyPress('" + strBtnCloseId + "');");
        if (!Page.IsPostBack)
        {
            txtContractCode.Text = codeFilter;
            ShowData(true);
        }
    }
    private void ShowData(bool pReSearch)
    {
        //IssuerBO objIssuerBO = new IssuerBO();

        if ((!Page.IsPostBack) && (Request.QueryString.Get("Page") != null))
        {
            _CurrentPage = int.Parse("0" + Request.QueryString.Get("Page"));
            txtShareHolderCode.Text = Request.QueryString.Get("ShareHolderCode");
            txtShareHolderName.Text = Request.QueryString.Get("ShareHolderName");
        }
        else if (!pReSearch && (ddlSelectPage.SelectedValue != null))
            _CurrentPage = int.Parse("0" + ddlSelectPage.SelectedValue);

        if (_CurrentPage == 0)
            _CurrentPage = 1;

        // Gán chiều cao mặc định cho dòng header
        tblData.Rows[0].Height = csHTML.HeaderRowHeight;

        BondContractBO objBondContractBO = new BondContractBO();
        List<T_SH_PORTAL_CONTRACT_SELL> objListContractVpsSell = objBondContractBO.get_ModalListAllSellOutright(
                                                                                            strIssuerCode //pStrIssuerCode
                                                                                            , "" // pStrStatus
                                                                                            , "" // pStrShmCustomerCode
                                                                                            , txtShareHolderName.Text.Trim() // pStrShmCustomerName
                                                                                            , txtCardID.Text.Trim() // pStrShmCardId
                                                                                            , txtShareHolderCode.Text.Trim() // pStrShCode
                                                                                            , txtContractCode.Text.Trim() // pStrContractNo
                                                                                            , txtVPSAccount.Text.Trim()
                                                                                            , "" // pStrUserCode
                                                                                            , _CurrentPage //pPageNo
                                                                                            , _PageSize //pPageSize
                                                                                            );

        if (objListContractVpsSell.Count > 0)
        {
            foreach (T_SH_PORTAL_CONTRACT_SELL objContractVpsSell in objListContractVpsSell)
            {
                TableRow trRow = new TableRow();
                string strLink = "<a href=javascript:" + "btnRowOnClick('" + strTxtReceiver + "','"
                + Uri.EscapeUriString(objContractVpsSell.C_CONTRACT_NO) + "^" + Uri.EscapeUriString(objContractVpsSell.C_APPENDIX_NO) + "','"
                + strBtnCloseId + "');" + ">$$$</a>";

                trRow.Cells.Add(objHTML.CreateNewCell(strLink.Replace("$$$", objContractVpsSell.C_CONTRACT_NO), false, HorizontalAlign.Center));
                trRow.Cells.Add(objHTML.CreateNewCell(strLink.Replace("$$$", objContractVpsSell.C_APPENDIX_NO), false, HorizontalAlign.Center));
                trRow.Cells.Add(objHTML.CreateNewCell(strLink.Replace("$$$", objContractVpsSell.C_SHM_CUSTOMER_NAME), false, HorizontalAlign.Left));
                trRow.Cells.Add(objHTML.CreateNewCell(strLink.Replace("$$$", objContractVpsSell.C_BO_CUST_CARD_ID), false, HorizontalAlign.Center));
                trRow.Cells.Add(objHTML.CreateNewCell(strLink.Replace("$$$", objContractVpsSell.C_TERM), false, HorizontalAlign.Center));
                trRow.Cells.Add(objHTML.CreateNewCell(strLink.Replace("$$$", objContractVpsSell.C_UNIT.ToString()), false, HorizontalAlign.Center));

                if (tblData.Rows.Count % 2 != 0)
                    trRow.CssClass = "odd_row";//csHTML.EvenColor;
                else
                    trRow.CssClass = "round_row";//csHTML.OddColor;

                trRow.Height = csHTML.ContentRowHeight;

                tblData.Rows.Add(trRow);
            }
        }

        if (pReSearch)
        {
            // Load lại mục chọn trang
            int _PageNumber = objBondContractBO.TotalRecord / _PageSize + (objBondContractBO.TotalRecord % _PageSize != 0 ? 1 : 0);

            ddlSelectPage.Items.Clear();

            for (int i = 1; i <= _PageNumber; i++)
            {
                ListItem lsiNewItem = new ListItem();

                lsiNewItem.Value = i.ToString();
                lsiNewItem.Text = i.ToString();

                ddlSelectPage.Items.Add(lsiNewItem);
            }
        }

        if (_CurrentPage != 1)
            ddlSelectPage.SelectedValue = _CurrentPage.ToString();

    }
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        ShowData(true);
    }
    protected void btnFirstPage_Click(object sender, EventArgs e)
    {
        if (ddlSelectPage.SelectedValue != "")
        {
            if (int.Parse(ddlSelectPage.SelectedValue) > 1)
            {
                ddlSelectPage.SelectedValue = "1";
            }
        }

        ShowData(false);
    }
    protected void btnPreviousPage_Click(object sender, EventArgs e)
    {
        if (ddlSelectPage.SelectedValue != "")
        {
            if (int.Parse(ddlSelectPage.SelectedValue) > 1)
            {
                ddlSelectPage.SelectedValue = "" + (int.Parse(ddlSelectPage.SelectedValue) - 1);
            }
        }

        ShowData(false);
    }
    protected void btnNextPage_Click(object sender, EventArgs e)
    {
        if (ddlSelectPage.SelectedValue != "")
        {
            if (int.Parse(ddlSelectPage.SelectedValue) < ddlSelectPage.Items.Count)
            {
                ddlSelectPage.SelectedValue = "" + (int.Parse(ddlSelectPage.SelectedValue) + 1);
            }
        }

        ShowData(false);
    }
    protected void btnLastPage_Click(object sender, EventArgs e)
    {
        if (ddlSelectPage.SelectedValue != "")
        {
            if (int.Parse(ddlSelectPage.SelectedValue) < ddlSelectPage.Items.Count)
            {
                ddlSelectPage.SelectedValue = ddlSelectPage.Items.Count.ToString();
            }
        }

        ShowData(false);
    }
    protected void ddpSelectPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        ShowData(false);
    }
}
