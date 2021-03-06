﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Models.Menu> menus = context.Menus.ToList();
            return View(menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addMenuViewModel);
            }
            else
            {
                Menu newMenu = new Menu();
                newMenu.Name = addMenuViewModel.Name;
                context.Menus.Add(newMenu);
                context.SaveChanges();
                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }
        }

        public IActionResult ViewMenu(int id)
        {
            Menu newMenu = new Menu();
            newMenu = context.Menus.Single(m => m.ID == id);

            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel(items, newMenu);
            return View(viewMenuViewModel);
        }

        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);
            List<Cheese> cheeses = context.Cheeses.ToList();
            return View(new AddMenuItemViewModel(menu, cheeses));
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addMenuItemViewModel);
            }
            else
            {
                IList<CheeseMenu> existingItems = context.CheeseMenus
                    .Where(cm => cm.CheeseID == addMenuItemViewModel.CheeseID)
                    .Where(cm => cm.MenuID == addMenuItemViewModel.MenuID).ToList();
                if (existingItems.Count == 0)
                {
                    CheeseMenu cheeseMenu = new CheeseMenu();
                    cheeseMenu.CheeseID = addMenuItemViewModel.CheeseID;
                    cheeseMenu.MenuID = addMenuItemViewModel.MenuID;
                    context.CheeseMenus.Add(cheeseMenu);
                    context.SaveChanges();

                    return Redirect("/Menu/ViewMenu/" + cheeseMenu.MenuID);
                }
                else
                {
                    return View(addMenuItemViewModel);
                }
            }

        }

  
    }
}
