using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArtVenue.Data;
using ArtVenue.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ArtVenue.ViewModels;
using System.Globalization;

namespace ArtVenue.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public GroupsController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }


        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Group> groupsCreated = new List<Group>();
            List<Group> groupsJoined = new List<Group>();
            if (_db.Groups.Where(x => x.CreatorId == user.Id).Any())
            {
                groupsCreated = await _db.Groups.Where(x => x.CreatorId == user.Id).ToListAsync();
            }
            int[] groupsCreatedIds = groupsCreated.Select(x => x.Id).ToArray();
            foreach (var membership in _db.Groups_Members.Where(x=>x.MemberId == user.Id))
            {
                if (!groupsCreatedIds.Contains(membership.GroupId))
                {
                    Group groupJoined = await _db.Groups.FindAsync(membership.GroupId);
                    groupsJoined.Add(groupJoined);
                }
            }
            GroupsIndexViewModel data = new GroupsIndexViewModel()
            {
                GroupsCreated = groupsCreated,
                GroupsJoined = groupsJoined
            };
            return View(data);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group group)
        {
            var user = await _userManager.GetUserAsync(User);
            group.CreatorId = user.Id;
            try
            {
                _db.Groups.Add(group);
                _db.Groups_Members.Add(new Groups_Members()
                {
                    MemberId = user.Id,
                    Group = group,
                    JoinedDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture)
                }) ;
                await _db.SaveChangesAsync();
                return RedirectToAction("index");
            }
            catch
            {
                return View(group);
            }

        }
    
        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null)
            {
                return Forbid();
            }
            if (!IsCreatorOf(group))
            {
                return Unauthorized();
            }
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGroup(Group group)
        {
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }
            try
            {
                _db.Update(group);
                await _db.SaveChangesAsync();
                return RedirectToAction("index");
            }
            catch
            {
                return View(group);
            }
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group==null)
            {
                return NotFound();
            }
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _db.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }
            _db.Groups.Remove(group);
            await _db.SaveChangesAsync();
            return RedirectToAction("index");
        }
        public async Task<IActionResult> Members(int id)
        {
            var group = await _db.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }
            List<Groups_Members> memberships = new List<Groups_Members>();
            if (await _db.Groups_Members.Where(x => x.GroupId == id).AnyAsync())
            {
                foreach (var membership in _db.Groups_Members.Where(x => x.GroupId == id).ToList())
                {
                    if (membership.MemberId!=group.CreatorId)
                    {
                        membership.Member = await _userManager.FindByIdAsync(membership.MemberId);
                        memberships.Add(membership);
                    }
                }
            }
            GroupsMembersViewModel data = new GroupsMembersViewModel() {
                GroupName = group.GroupName,
                Memberships = memberships
            };
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveMember(int groupId, string userId)
        {
            var group = await _db.Groups.FindAsync(groupId);
            if (group == null)
            {
                return NotFound();
            }
            if (!IsCreatorOf(group) || group.CreatorId == userId)
            {
                return Forbid();
            }
            var membership = await _db.Groups_Members.Where(x => x.GroupId == groupId && x.MemberId == userId).FirstAsync();
            _db.Groups_Members.Remove(membership);
            await _db.SaveChangesAsync();
            return RedirectToAction("members", new { Id = groupId });
        }
        public async Task<IActionResult> Requests(int id)
        {
            var group = await _db.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }
            List<Groups_Requests> requests = new List<Groups_Requests>();
            if (await _db.Groups_Requests.Where(x => x.GroupId == id).AnyAsync())
            {
                foreach (var request in _db.Groups_Requests.Where(x => x.GroupId == id))
                {
                    request.Member = await _userManager.FindByIdAsync(request.MemberId);
                    requests.Add(request);
                }
            }
            GroupsRequestsViewModel data = new GroupsRequestsViewModel()
            {
                GroupName = group.GroupName,
                Requests = requests
            };
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int groupId, string userId)
        {
            var group = await _db.Groups.FindAsync(groupId);

            if (group == null)
            {
                return NotFound();
            }
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }
            var request = await _db.Groups_Requests.Where(x => x.GroupId == groupId && x.MemberId == userId).FirstAsync();
            _db.Groups_Requests.Remove(request);
            await _db.SaveChangesAsync();
            return RedirectToAction("requests", new {Id=groupId });
        }
        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int groupId, string userId)
        {
            var group = await _db.Groups.FindAsync(groupId);

            if (group == null)
            {
                return NotFound();
            }
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }
            var request = await _db.Groups_Requests.Where(x => x.GroupId == groupId && x.MemberId == userId).FirstAsync();
            _db.Groups_Requests.Remove(request);
            _db.Groups_Members.Add(new Groups_Members()
            {
                GroupId = groupId,
                MemberId = userId,
                JoinedDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture)
        }) ;
            await _db.SaveChangesAsync();
            return RedirectToAction("requests", new { Id = groupId });
        }
        private bool IsCreatorOf(Group group)
        {
            return group.CreatorId == _userManager.GetUserId(User);
        }
    }
}
