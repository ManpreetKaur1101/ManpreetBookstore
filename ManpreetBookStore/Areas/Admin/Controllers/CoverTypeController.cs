﻿using Dapper;
using ManpreetBooks.DataAccess.Repository.IRepository;
using ManpreetBooks.Models;
using ManpreetBooks.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManpreetBookStore.Areas.Admin.Controllers

{
    [Area("Admin")]
    public class CoverTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CoverTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Upsert(int? id)
    {
        CoverType coverType = new CoverType();
        if (id == null)
        {
            // this is for create
            return View(coverType);
        }
        // this is for edit
        var parameter = new DynamicParameters();
        parameter.Add("@Id", id);
        coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
        if (coverType == null)
        {
            return NotFound();
        }
        return View(coverType);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(CoverType coverType)
    {
        if (ModelState.IsValid)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Name", coverType.Name);

            if (coverType.id == 0)
            {
                _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
            }
            else
            {
                parameter.Add("@Id", coverType.id);
                _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        return View(coverType);
    }

    #region API CALLS

    public IActionResult GetAll()
    {
        var allObj = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);
        return Json(new { data = allObj });
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var parameter = new DynamicParameters();
        parameter.Add("@Id", id);
        var objFromDb = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
        if (objFromDb == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}
}
