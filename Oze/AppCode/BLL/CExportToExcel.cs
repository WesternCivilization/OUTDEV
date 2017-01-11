using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace EzOpen.AppCode.BLL
{
    public class CExportToExcel
    {       
    }

    public abstract class DataGridExporterBase
    {
        /// <summary>
        /// Holds a reference to the datagrid being exported
        /// </summary>

        protected DataGrid MyDataGrid;
        /// <summary>
        /// Holds a reference to the page where the datagrid locates
        /// </summary>

        protected Page CurrentPage;
        /// <summary>
        /// Overloaded. Initializes a new instance of the DataGridExporterBase class.
        /// </summary>
        /// <param name="dg">The datagrid to be exported</param>
        /// <param name="pg">The page to which the datagrid is to be exported</param>
        public DataGridExporterBase(DataGrid dg, Page pg)
        {
            MyDataGrid = dg;
            CurrentPage = pg;
        }

        /// <summary>
        /// Overloaded. Initializes a new instance of the DataGridExporterBase class.
        /// </summary>
        /// <param name="dg">The datagrid to be exported</param>
        public DataGridExporterBase(DataGrid dg)
            : this(dg, dg.Page)
        {
        }

        /// <summary>
        /// Exports the current datagrid
        /// </summary>
        public abstract void Export();
    }



    /// <summary>
    /// Exports a datagrid to a excel file. 
    /// </summary>
    /// <requirements>Microsoft Excel 97 or above should be installed on the client machine in order to make 
    /// this function work
    /// </requirements>
    public class DataGridExcelExporter : DataGridExporterBase
    {

        /// <summary>
        /// CSS file for decoration, se it if any or dont use it
        /// </summary>

        private const string MY_CSS_FILE = "../MDF.css";
        /// <summary>
        /// Overloaded. Initializes a new instance of the DataGridExcelExporter class.
        /// </summary>
        /// <param name="dg">The datagrid to be exported</param>
        /// <param name="pg">The page to which the datagrid is to be exported</param>
        public DataGridExcelExporter(DataGrid dg, Page pg)
            : base(dg, pg)
        {
        }

        /// <summary>
        /// Overloaded. Initializes a new instance of the DataGridExcelExporter class.
        /// </summary>
        /// <param name="dg">The datagrid to be exported</param>
        public DataGridExcelExporter(DataGrid dg)
            : base(dg)
        {
        }

        /// <summary>
        /// Overloaded. Exports a datagrid to an excel file, the title of which is empty
        /// </summary>
        public override void Export()
        {
            Export(String.Empty);
        }

        /// <summary>
        /// Renders the html text before the datagrid.
        /// </summary>
        /// <param name="writer">A HtmlTextWriter to write html to output stream</param>
        protected virtual void FrontDecorator(HtmlTextWriter writer)
        {
            writer.WriteFullBeginTag("HTML");
            writer.WriteFullBeginTag("meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"");
            writer.WriteFullBeginTag("Head");
            writer.RenderBeginTag(HtmlTextWriterTag.Style);
            writer.Write("<!--");

            if (File.Exists(CurrentPage.MapPath(MY_CSS_FILE)))
            {
                StreamReader sr = File.OpenText(CurrentPage.MapPath(MY_CSS_FILE));
                string input = null;
                while ((input == sr.ReadLine()))
                {
                    writer.WriteLine(input);
                }
                sr.Close();
            }
            writer.Write("-->");
            writer.RenderEndTag();
            writer.WriteEndTag("Head");
            writer.WriteFullBeginTag("Body");
        }
        /// <summary>
        /// Renders the html text after the datagrid.
        /// </summary>
        /// <param name="writer">A HtmlTextWriter to write html to output stream</param>
        protected virtual void RearDecorator(HtmlTextWriter writer)
        {
            writer.WriteEndTag("Body");
            writer.WriteEndTag("HTML");
        }

        /// <summary>
        /// Exports the datagrid to an Excel file with the name of the datasheet provided by the passed in parameter
        /// Exports all field visiable = true
        /// </summary>
        /// <param name="reportName">Name of the datasheet.
        /// </param>
        public void Export(string reportName)
        {
            MyDataGrid.AllowPaging = false;
            //MyDataGrid.AllowCustomPaging = false;
            //MyDataGrid.DataBind();
            ClearChildControls(MyDataGrid);
            AddSpaceControls(MyDataGrid);
            MyDataGrid.EnableViewState = false;
            //Gets rid of the viewstate of the control. The viewstate may make an excel file unreadable.

            CurrentPage.Response.Clear();
            CurrentPage.Response.Buffer = true;

            //This will make the browser interpret the output as an Excel file
            //CurrentPage.Response.AddHeader( "Content-Disposition", "filename="+reportName);
            CurrentPage.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + reportName + ".xls\"");
            CurrentPage.Response.ContentType = "application/vnd.ms-excel";

            //Format 
            //header 
            MyDataGrid.HeaderStyle.Font.Bold = true;
            MyDataGrid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            MyDataGrid.HeaderStyle.BackColor = System.Drawing.Color.DarkGreen;
            MyDataGrid.HeaderStyle.Height = 26;

            //Prepares the html and write it into a StringWriter
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            FrontDecorator(htmlWriter);
            MyDataGrid.RenderControl(htmlWriter);
            RearDecorator(htmlWriter);

            //Write the content to the web browser
            CurrentPage.Response.Write(stringWriter.ToString());
            CurrentPage.Response.End();
        }

        /// <summary>
        /// Exports the datagrid to an Excel file with the name of the datasheet provided by the passed in parameter
        /// Exports all field visiable = true/false
        /// </summary>
        /// <param name="reportName">Name of the datasheet.
        /// </param>
        public void ExportAllField(string reportName)
        {
            MyDataGrid.AllowPaging = false;
            //MyDataGrid.AllowCustomPaging = false;
            //MyDataGrid.DataBind();
            ClearChildControls(MyDataGrid);
            AddSpaceControls(MyDataGrid);
            MyDataGrid.EnableViewState = false;
            //Gets rid of the viewstate of the control. The viewstate may make an excel file unreadable.

            CurrentPage.Response.Clear();
            CurrentPage.Response.Buffer = true;

            //This will make the browser interpret the output as an Excel file
            //CurrentPage.Response.AddHeader( "Content-Disposition", "filename="+reportName);
            CurrentPage.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + reportName + ".xls\"");
            CurrentPage.Response.ContentType = "application/vnd.ms-excel";

            //Format 
            //header 
            MyDataGrid.HeaderStyle.Font.Bold = true;
            MyDataGrid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            MyDataGrid.HeaderStyle.BackColor = System.Drawing.Color.DarkGreen;
            MyDataGrid.HeaderStyle.Height = 26;

            //Prepares the html and write it into a StringWriter
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            FrontDecorator(htmlWriter);

            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                column.Visible = true;
            }

            MyDataGrid.RenderControl(htmlWriter);
            RearDecorator(htmlWriter);

            //Write the content to the web browser
            CurrentPage.Response.Write(stringWriter.ToString());
            CurrentPage.Response.End();
        }

        /// <summary>
        /// Exports the datagrid to an Word file with the name of the datasheet provided by the passed in parameter
        /// </summary>
        /// <param name="reportName">Name of the datasheet.
        /// </param>
        public virtual void ExportWord(string reportName)
        {
            MyDataGrid.AllowPaging = false;
            //MyDataGrid.AllowCustomPaging = false;
            //MyDataGrid.DataBind();
            ClearChildControls(MyDataGrid);
            AddSpaceControls(MyDataGrid);
            MyDataGrid.EnableViewState = false;
            //Gets rid of the viewstate of the control. The viewstate may make an excel file unreadable.

            CurrentPage.Response.Clear();
            CurrentPage.Response.Buffer = true;

            //This will make the browser interpret the output as an Excel file
            //CurrentPage.Response.AddHeader( "Content-Disposition", "filename="+reportName);
            CurrentPage.Response.AddHeader("Content-Disposition", "attachment; filename=export.doc");
            CurrentPage.Response.ContentType = "application/vnd.ms-word";


            //Prepares the html and write it into a StringWriter
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            FrontDecorator(htmlWriter);
            MyDataGrid.RenderControl(htmlWriter);
            RearDecorator(htmlWriter);

            //Write the content to the web browser
            CurrentPage.Response.Write(stringWriter.ToString());
            CurrentPage.Response.End();
        }

        /// <summary>
        /// Exports the datagrid to an Word file with the name of the datasheet provided by the passed in parameter
        /// </summary>
        /// <param name="reportName">Name of the datasheet.
        /// </param>
        public virtual void ExportHTMLWord(string reportName, Control con)
        {
            //			ClearChildControls(MyDataGrid);
            //			AddSpaceControls(MyDataGrid);
            //			MyDataGrid.EnableViewState = false;//Gets rid of the viewstate of the control. The viewstate may make an excel file unreadable.
            this.RecursiveClear(con);

            CurrentPage.Response.Clear();
            CurrentPage.Response.Buffer = true;

            //This will make the browser interpret the output as an Excel file
            //CurrentPage.Response.AddHeader( "Content-Disposition", "filename="+reportName);
            CurrentPage.Response.AddHeader("Content-Disposition", "attachment; filename=export.doc");
            CurrentPage.Response.ContentType = "application/vnd.ms-word";


            //Prepares the html and write it into a StringWriter
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            FrontDecorator(htmlWriter);
            con.RenderControl(htmlWriter);
            RearDecorator(htmlWriter);

            //Write the content to the web browser
            CurrentPage.Response.Write(stringWriter.ToString());
            CurrentPage.Response.End();
        }



        /// <summary>
        /// Iterates a control and its children controls, ensuring they are all LiteralControls
        /// <remarks>
        /// Only LiteralControl can call RenderControl(System.Web.UI.HTMLTextWriter htmlWriter) method. Otherwise 
        /// a runtime error will occur. This is the reason why this method exists.
        /// </remarks>
        /// </summary>
        /// <param name="control">The control to be cleared and verified</param>
        private void RecursiveClear(System.Web.UI.Control control)
        {
            for (int i = control.Controls.Count - 1; i >= 0; i += -1)
            {
                //Clears children controls
                RecursiveClear(control.Controls[i]);
            }

            //
            //If it is a LinkButton, convert it to a LiteralControl
            //
            if (control is LinkButton)
            {
                LiteralControl literal = new LiteralControl();
                control.Parent.Controls.Add(literal);
                literal.Text = ((LinkButton)control).Text;
                control.Parent.Controls.Remove(control);
            }
            else if (control is Button)
            {
                //We don't need a button in the excel sheet, so simply delete it
                control.Parent.Controls.Remove(control);

            }
            else if (control is ListControl)
            {
                //If it is a ListControl, copy the text to a new LiteralControl
                LiteralControl literal = new LiteralControl();
                control.Parent.Controls.Add(literal);
                try
                {
                    literal.Text = ((ListControl)control).SelectedItem.Text;
                }
                catch
                {
                }

                control.Parent.Controls.Remove(control);
            }
            else if (control is CheckBox)
            {
                control.Parent.Controls.Remove(control);
            }
            else if (control is TextBox)
            {
                LiteralControl literal = new LiteralControl();
                control.Parent.Controls.Add(literal);
                try
                {
                    literal.Text = ((TextBox)control).Text;
                }
                catch
                {
                }
                control.Parent.Controls.Remove(control);
            }
            else if (control is ImageButton)
            {
                control.Parent.Controls.Remove(control);
            }
            else if (control is Image)
            {
                control.Parent.Controls.Remove(control);
            }

            //You may add more conditions when necessary

            return;
        }


        /// <summary>
        /// Clears the child controls of a Datagrid to make sure all controls are LiteralControls
        /// </summary>
        /// <param name="dg">Datagrid to be cleared and verified</param>
        protected void ClearChildControls(DataGrid dg)
        {

            for (int i = dg.Columns.Count - 1; i >= 0; i += -1)
            {
                DataGridColumn column = dg.Columns[i];
                if (column is ButtonColumn)
                {
                    dg.Columns.Remove(column);
                }
            }

            this.RecursiveClear(dg);

        }

        protected void AddSpaceControls(DataGrid dg)
        {
            for (int i = 0; i <= dg.Items.Count - 1; i++)
            {
                TableCellCollection cells = (TableCellCollection)dg.Items[i].Cells;
                foreach (TableCell cell in cells)
                {
                    if (!string.IsNullOrEmpty(cell.Text.ToString().Trim()))
                    {
                        cell.Text = cell.Text;
                    }
                    string a = cell.Text;
                }
            }
        }

    }

    /// <summary>
    /// HTML Encodes an entire DataGrid. 
    /// It iterates through each cell in the TableRow, ensuring that all 
    /// the text being displayed is HTML Encoded, irrespective of whether 
    /// they are just plain text, buttons, hyperlinks, multiple controls etc..
    /// </summary>
    public class CellFormater
    {
        /// <summary>
        /// Constructs an instance of the CellFormater class.
        /// </summary>
        //
        // TODO: Add constructor logic here
        //

        public CellFormater()
        {
        }

        /// <summary>
        /// Method that HTML Encodes an entire DataGrid. 
        /// It iterates through each cell in the TableRow, ensuring that all 
        /// the text being displayed is HTML Encoded, irrespective of whether 
        /// they are just plain text, buttons, hyperlinks, multiple controls etc..
        /// <seealso cref="DataGrid.ItemDataBound">DataGrid.ItemDataBound Event</seealso>
        /// </summary>
        /// <param name="item">
        /// The DataGridItem that is currently being bound in the calling Web 
        /// Page's DataGrid.ItemDataBound Event.
        /// </param>
        /// <remarks>
        /// This method should be called from the 
        /// <c>DataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)</c> 
        /// event in the respective Web View Codebehind.
        /// </remarks>
        /// <example>
        ///          We want to HTMLEncode a complete DataGrid (all columns and all 
        ///          rows that may/do contain characters that will require encoding 
        ///          for display in HTML) called dgIssues.
        ///          Use the following code for the ItemDataBound Event:
        ///          <code>
        ///               private void dgIssues_ItemDataBound(object sender, DataGridItemEventArgs e)
        ///               {
        ///                    WebMethod wm = new WebMethod();
        ///                    wm.DataGrid_ItemDataBound_HTMLEncode((DataGridItem) e.Item);
        ///               }//dgIssues_ItemDataBound
        ///          </code>
        /// </example>
        public void AdHocHTMLEncode(DataGridItem item)
        {
            bool doHTMLEncode = false;
            switch (item.ItemType)
            {
                case ListItemType.Item:

                    //The following case statements are in ascending TableItemStyle order.
                    //See ms-help://MS.VSCC/MS.MSDNVS/cpref/html/frlrfsystemwebuiwebcontrolsdatagridclassitemstyletopic.htm for details.
                    doHTMLEncode = true;
                    break; // TODO: might not be correct. Was : Exit Select
                case ListItemType.AlternatingItem:
                    //ListItemType.Item
                    doHTMLEncode = true;
                    break; // TODO: might not be correct. Was : Exit Select6
                case ListItemType.SelectedItem:
                    //ListItemType.AlternatingItem
                    doHTMLEncode = true;
                    break; // TODO: might not be correct. Was : Exit Select
                case ListItemType.EditItem:
                    //ListItemType.SelectedItem					
                    //These should not be prone to this as TextBoxes aren't.
                    doHTMLEncode = false;
                    break; // TODO: might not be correct. Was : Exit Select
                case ListItemType.Header:
                    //ListItemType.EditItem

                    //The remainder are the other ListItemTypes that are non-Data-bound.
                    //We might have specified Headers like "<ID>".
                    doHTMLEncode = true;
                    break; // TODO: might not be correct. Was : Exit Select
                case ListItemType.Footer:
                    //ListItemType.Header
                    //Similarly for the Footer as with the Header.
                    doHTMLEncode = true;

                    break; // TODO: might not be correct. Was : Exit Select
                case ListItemType.Pager:
                    //ListItemType.Footer
                    //With just numbers or buttons, none is required.
                    //However, for buttons, this is not strictly true as you 
                    //need to specify the text on the buttons. But the Property 
                    //Builder for the DataGrid hints in its defaults that these 
                    //need to be HTMLencoded anyway.
                    doHTMLEncode = false;
                    break; // TODO: might not be correct. Was : Exit Select
                case ListItemType.Separator:
                    //ListItemType.Pager
                    doHTMLEncode = false;
                    break; // TODO: might not be correct. Was : Exit Select
                default:
                    //ListItemType.Separator

                    //This will never be executed as all ItemTypes are listed above.
                    break; // TODO: might not be correct. Was : Exit Select
                //default
            }
            //switch
            if (doHTMLEncode)
            {
                //Encode the cells dependent on the type of content 
                //within (e.g. BoundColumn, Hyperlink), taking into account 
                //that there may be more than one (or even zero) control in 
                //each cell.
                TableCellCollection cells = (TableCellCollection)item.Cells;
                foreach (TableCell cell in cells)
                {
                    if (cell.Controls.Count != 0)
                    {
                        foreach (System.Web.UI.Control ctrl in cell.Controls)
                        {
                            if (ctrl is Button)
                            {
                                Button btn = (Button)ctrl;
                                btn.Text = HttpUtility.HtmlEncode(btn.Text);
                            }
                            else if (ctrl is HyperLink)
                            {
                                //if
                                HyperLink hyp = (HyperLink)ctrl;
                                //hyp.NavigateUrl = HttpUtility.UrlEncode(hyp.NavigateUrl);
                                hyp.Text = HttpUtility.HtmlEncode(hyp.Text);
                            }
                            else if (ctrl is LinkButton)
                            {
                                //else if
                                LinkButton lb = (LinkButton)ctrl;
                                lb.Text = HttpUtility.HtmlEncode(lb.Text);
                            }
                            else if (ctrl is Label)
                            {
                                //else if
                                // this check is for to change the forecolor of REJECTED activities to red
                                Label objL = (Label)ctrl;
                                if (objL.Text == "REJECTED")
                                {
                                    objL.ForeColor = System.Drawing.Color.Red;
                                }
                                //else if
                            }
                            //foreach
                        }
                    }
                    else
                    {
                        //if
                        //The cell is a BoundColumn.
                        if (cell.Text.ToLower().Trim() != "&nbsp;")
                        {
                            cell.Text = HttpUtility.HtmlEncode(cell.Text);

                        }
                        //else
                    }
                    //foreach
                }
            }
            //if
        }
        //DataGrid_ItemDataBound_HTMLEncode
    }
}