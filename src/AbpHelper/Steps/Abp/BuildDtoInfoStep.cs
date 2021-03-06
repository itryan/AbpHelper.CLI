﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.AbpHelper.Commands.Generate.Crud;
using EasyAbp.AbpHelper.Extensions;
using EasyAbp.AbpHelper.Models;
using EasyAbp.AbpHelper.Steps.Common;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Scripting.JavaScript;
using Elsa.Services.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace EasyAbp.AbpHelper.Steps.Abp
{
    public class BuildDtoInfoStep : Step
    {        
        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            var entityInfo = context.GetVariable<EntityInfo>("EntityInfo");
            var option = context.GetVariable<object>("Option") as CrudCommandOption;

            try
            {
                string[] actionNames = new string[] { string.Empty, string.Empty, string.Empty};                

                if (option != null && option.SeparateDto)
                {
                    actionNames[1] = "Create";
                    actionNames[2] = "Update";
                }
                else
                {
                    actionNames[1] = "CreateUpdate";
                    actionNames[2] = actionNames[1];
                }

                string[] typeNames = new string[actionNames.Length];

                var useEntityPrefix = option != null && option.EntityPrefixDto;
                var dtoSubfix = option?.DtoSuffix ?? "Dto";

                for (int i = 0; i < typeNames.Length; i++)
                {
                    typeNames[i] = useEntityPrefix
                        ? $"{entityInfo.Name}{actionNames[i]}{dtoSubfix}"
                        : $"{actionNames[i]}{entityInfo.Name}{dtoSubfix}";
                }
                
                DtoInfo dtoInfo = new DtoInfo(typeNames[0], typeNames[1], typeNames[2]);
               
                context.SetLastResult(dtoInfo);
                context.SetVariable("DtoInfo", dtoInfo);
                LogOutput(() => dtoInfo);

                return Done();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Building DTO info failed.");
                if (e is ParseException pe)
                    foreach (var error in pe.Errors)
                        Logger.LogError(error);
                throw;
            }
        }
    }
}