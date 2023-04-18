using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Atelier.Data;
using Atelier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Atelier.Authorizations;

namespace Atelier.Controllers
{
    public class VetementsController : Controller
    {
        private readonly ApplicationDbContext context;
        private IAuthorizationService AuthorizationService { get; }
        private UserManager<IdentityUser> UserManager { get; }

        public VetementsController(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager) 
        {
            this.context = context;
            AuthorizationService = authorizationService;
            UserManager = userManager; 
        }

        // GET: Vetements
        public async Task<IActionResult> Index() 
        { 
            if (context.Vetement == null) 
                return NotFound(); 

            var vetements = from v in context.Vetement select v; 
            var isAuthorized = User.IsInRole(AuthorizationConstants.VetementAdministratorsRole); 
            var currentUserId = UserManager.GetUserId(User); 
            if (!isAuthorized)
                vetements = vetements.Where(v => v.ProprietaireId == currentUserId);
            return View(await vetements.ToListAsync()); }

        // GET: Vetements/Details/5
        public async Task<IActionResult> Details(int? id)
        { 
            if (id == null || context.Vetement == null)
            { 
                return NotFound(); 
            } 
            var v = await context.Vetement.FirstOrDefaultAsync(m => m.VetementId == id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, v, VetementOperations.Read);
            if (!isAuthorized.Succeeded) 
                return Forbid();
            return View(v);
        }

        // GET: Vetements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vetements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VetementId,Nom,Description,DateObtention,Type,Image")] Vetement vetement)
        {
            if (ModelState.IsValid)
            {
                vetement.ProprietaireId = UserManager.GetUserId(User);
                var isAuthorized = await AuthorizationService.AuthorizeAsync(
                    User, vetement, VetementOperations.Create);

                if (!isAuthorized.Succeeded)
                    return Forbid();

                context.Add(vetement);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vetement);
        }

        // GET: Vetements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Vetement == null)
            {
                return NotFound();
            }

            var vetement = await context.Vetement.FindAsync(id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                   User, vetement, VetementOperations.Update);

            if (!isAuthorized.Succeeded)
                return Forbid();

            if (vetement == null)
                return NotFound();
            return View(vetement);
        }

        // POST: Vetements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VetementId,Nom,Description,DateObtention,Type,Image")] Vetement vetement)
        {
           

            if (id != vetement.VetementId)
            {
                return NotFound();
            }

            var v = await context.Vetement.AsNoTracking().FirstOrDefaultAsync(m => m.VetementId == id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, v, VetementOperations.Update);

            vetement.ProprietaireId = v.ProprietaireId;

            if (!isAuthorized.Succeeded)
                return Forbid();
            if (v == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(vetement);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VetementExists(vetement.VetementId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vetement);
        }

        // GET: Vetements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Vetement == null)
            {
                return NotFound();
            }

            var v = await context.Vetement
                .FirstOrDefaultAsync(m => m.VetementId == id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, v, VetementOperations.Delete);

            if (!isAuthorized.Succeeded)
                return Forbid();
            if (v == null)
                return NotFound();

            return View(v);
        }

        // POST: Vetements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Vetement == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vetement'  is null.");
            }
            var v = await context.Vetement.FindAsync(id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, v, VetementOperations.Delete);

            if (!isAuthorized.Succeeded)
                return Forbid();

            if (v == null)
                return NotFound();
                
            context.Vetement.Remove(v);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VetementExists(int id)
        {
          return (context.Vetement?.Any(e => e.VetementId == id)).GetValueOrDefault();
        }
    }
}
