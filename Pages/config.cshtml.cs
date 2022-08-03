using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ThingsReportIt.Pages
{
    public class ConfigModel : PageModel
    {
        [BindProperty]
        [Required]
        [Range(0, 1000)]
        public string Thingid { get; set; }

        [BindProperty]
        public string CameraScreenSize { get; set; }
        public List<SelectListItem> CameraScreenSizeList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "100", Text = "100%" },
            new SelectListItem { Value = "75", Text = "75%" },
            new SelectListItem { Value = "50", Text = "50%"  },
        };

        [BindProperty]
        public string CameraSelection { get; set; }
        public List<SelectListItem> CameraSelectionList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "default", Text = "Default" },
            new SelectListItem { Value = "user", Text = "User" },
            new SelectListItem { Value = "environment", Text = "Environment"  },
        };

        [BindProperty]
        public string JPGQuality { get; set; }
        public List<SelectListItem> JPGQualityList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "0.95", Text = "0.95" },
            new SelectListItem { Value = "0.75", Text = "0.75" },
            new SelectListItem { Value = "0.55", Text = "0.55" },

        };

        public void OnGet()
        {
            CameraSelection = Request.Cookies["CameraSelection"];
            if (CameraSelection == null)
            {
                CameraSelection = "default";
            }

            CameraScreenSize = Request.Cookies["CameraScreenSize"];
            if (CameraScreenSize == null)
            {
                CameraScreenSize = "100%";
            }

            JPGQuality = Request.Cookies["JPGQuality"];
            if (JPGQuality == null)
            {
                JPGQuality = "0.75";
            }

            Thingid = Request.Cookies["Thingid"];
            if (Thingid == null)
            {
                Thingid = "0";
            }

        }
        public IActionResult OnPost()
        {
            if (CameraSelection != null)
            {
                Response.Cookies.Append("CameraSelection", CameraSelection,
                    new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = false,
                        Expires = DateTime.Now.AddMonths(12)
                    }
                );
            }

            if (CameraScreenSize != null)
            {
                Response.Cookies.Append("CameraScreenSize", CameraScreenSize,
                    new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = false,
                        Expires = DateTime.Now.AddMonths(12)
                    }
                );
            }

            if (JPGQuality != null)
            {
                Response.Cookies.Append("JPGQuality", JPGQuality,
                    new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = false,
                        Expires = DateTime.Now.AddMonths(12)
                    }
                );
            }

            if (Thingid != null)
            {
                Response.Cookies.Append("Thingid", Thingid,                    
                    new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = false,
                        Expires = DateTime.Now.AddMonths(12)
                    }
                );
            }

            return RedirectToPage("./Index");

        }

    }
}