using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ADMDATA
{
     public class SQLSelection
    {
         public string selectCubeTestToday()
         {
             string selectCube;
             selectCube = "SELECT [BarCode]" +
               ",[SpecimenRef]" +
               ",[JobRef]" +
               ",[ProjectCode]" +
               ",[Company]" +
               ",[ProjectTitle]" +
               ",[Size]" +
               ",[DateCast]" +
               ",[Age]" +
               ",[DateTest]" +
               ",[Grade]" +
               ",[SampleMass]" +
               ",[MaxLoad]" +
               ",[CubeID]" +
               ",[Remarks]" +
               " FROM [dbo].[vCubeTestAll] " +
               " ORDER BY ([BarCode])'"; 
             return selectCube;
         }


         public string selectCubeTestDone(DateTime selectDate)
        {
            string selectCube;
            selectCube = "SELECT [BarCode]" +
              ",[SpecimenRef]" +
              ",[JobRef]" +
              ",[ProjectCode]" +
              ",[Company]" +
              ",[ProjectTitle]" +
              ",[Size]" +
              ",[DateCast]" +
              ",[Age]" +
              ",[DateTest]" +
              ",[Grade]" +
              ",[SampleMass]" +
              ",[MaxLoad]" +
              ",[CubeID]" +
              ",[Remarks]" +
              " FROM [dbo].[vCubeTestAll] " +
              " WHERE ((CONVERT(Date,[DateTest]) " +
              "= CONVERT(DATETIME, '" + selectDate.ToString("yyyy-MM-dd") + "',102))" +
              " AND (([MaxLoad] IS NOT NULL) OR ([MaxLoad] > 0)))" +
              " ORDER BY ([SpecimenRef]);"; 
              //" ORDER BY ([BarCode]);"; 
            return selectCube;
        }
         public string selectCubeTestPending(DateTime selectDate)
        {
            string selectCube;
            selectCube = "SELECT [BarCode]" +
              ",[SpecimenRef]" +
              ",[JobRef]" +
              ",[ProjectCode]" +
              ",[Company]" +
              ",[ProjectTitle]" +
              ",[Size]" +
              ",[DateCast]" +
              ",[Age]" +
              ",[DateTest]" +
              ",[Grade]" +
              ",[SampleMass]" +
              ",[MaxLoad]" +
              ",[CubeID]" +
              ",[Remarks]" +
              " FROM [dbo].[vCubeTestAll] " +
              " WHERE ((CONVERT(Date,[DateTest]) " +
              "= CONVERT(DATETIME, '" + selectDate.ToString("yyyy-MM-dd") + "',102))" +
              " AND (([MaxLoad] IS NULL) OR ([MaxLoad] <= 0)))" +
              " ORDER BY ([SpecimenRef]);";
            return selectCube;
        }
        public string selectCubeTestAll( DateTime selectDate)
        {
            string selectCube;
            selectCube = "SELECT [BarCode]" +
              ",[SpecimenRef]" +
              ",[JobRef]" +
              ",[ProjectCode]" +
              ",[Company]" +
              ",[ProjectTitle]" +
              ",[Size]" +
              ",[DateCast]" +
              ",[Age]" +
              ",[DateTest]" +
              ",[Grade]" +
              ",[SampleMass]" +
              ",[MaxLoad]" +
              ",[CubeID]" +
              ",[Remarks]" +
              " FROM [dbo].[vCubeTestAll] " +
              " WHERE (CONVERT(Date,[DateTest]) " +
              "= CONVERT(DATETIME, '"  + selectDate.ToString("yyyy-MM-dd") + "',102))" +
              " ORDER BY ([SpecimenRef]);";
            
            return selectCube;
        }
    }
}
