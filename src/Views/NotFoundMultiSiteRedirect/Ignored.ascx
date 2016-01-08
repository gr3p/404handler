﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<CustomRedirect>>" %>
<%@ Import Namespace="EPiServer.Core" %>
<%@ Import Namespace="Knowit.NotFound.Core.CustomRedirects" %>

<div class="notfound">
       <div class="epi-formArea">
            <fieldset>
               <%=string.Format(EPiServer.Framework.Localization.LocalizationService.Current.GetString("/gadget/redirects/ignoredsuggestions"), Model.Count)%>
         
            </fieldset>
        </div>

       <% Html.RenderPartial("Menu", "Ignored"); %>
    <table class="epi-default">
        <thead>
            <tr>
                <th>
                    <label>
                        <%=EPiServer.Framework.Localization.LocalizationService.Current.GetString("/gadget/redirects/url")%></label>
                </th>
                <th>
                 <%=EPiServer.Framework.Localization.LocalizationService.Current.GetString("/gadget/redirects/unignore")%>
                </th>
            </tr>
        </thead>
        <%foreach (CustomRedirect m in Model) { %>
        <tr>
            <td class="redirect-longer">
                <%= Html.Encode(m.OldUrl)%>
            </td>
            <td class="shorter delete">
                <%= Html.ViewLink(
                        "",  // html helper
                        "Delete",  // title
                        "Unignore", // Action name
                        "epi-quickLinksDelete epi-iconToolbar-item-link epi-iconToolbar-delete", // css class
                        "Index",
                        new { url =  m.OldUrl })%>
            </td>
        </tr>
        <%} %>
    </table>
</div>