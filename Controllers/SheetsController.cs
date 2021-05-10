using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lumina.Data;
using Microsoft.AspNetCore.Mvc;

namespace LuminaAPI.Controllers
{
    [ApiController]
    [Route( "[controller]/{language}" )]
    public class SheetsController : ControllerBase
    {
        private readonly Lumina.GameData _lumina;
        
        // not thread safe, revision to be added later
        private static Dictionary< string, Type > _sheetNameToTypes = null!;
        private static MethodInfo _getSheetT = null!;

        public SheetsController( Lumina.GameData lumina )
        {
            _lumina = lumina;

            if( _sheetNameToTypes != null )
            {
                return;
            }
            
            _sheetNameToTypes = new();

            var assembly = typeof( Lumina.Excel.GeneratedSheets.Action ).Assembly;
            foreach( var type in assembly.GetTypes().Where( x => x.Namespace == typeof( Lumina.Excel.GeneratedSheets.Action ).Namespace ) )
            {
                _sheetNameToTypes[ type.Name.ToLowerInvariant() ] = type;
            }

            _getSheetT = typeof( Lumina.GameData)
                .GetMethods( BindingFlags.Instance | BindingFlags.Public )
                .FirstOrDefault( x => x.Name == "GetExcelSheet" && x.GetParameters().Any() );
        }
        
        [HttpGet("{sheetName}/{rowId}")]
        [HttpGet("{sheetName}/{rowId}/{subRowId}")]
        public ActionResult GetSheetRow( Language language, string sheetName, uint rowId, uint subRowId = UInt32.MaxValue, [FromQuery] int pretty = 0 )
        {
            sheetName = sheetName.ToLowerInvariant();
            if( !_sheetNameToTypes.TryGetValue( sheetName, out var sheetType ) )
            {
                return NotFound( "no typed sheet found with that name!" );
            }

            // revision to be added later
            var getSheetTyped = _getSheetT.MakeGenericMethod( sheetType );
            var sheet = getSheetTyped.Invoke( _lumina, new object[] { language } );

            var fn = sheet
                .GetType()
                .GetMethods( BindingFlags.Instance | BindingFlags.Public )
                .FirstOrDefault( x => x.Name == "GetRow" && x.GetParameters().Length == 2 );

            var data = fn.Invoke( sheet, new object[] { rowId, subRowId } );
            if( data != null )
            {
                return Ok( data );
            }

            return NotFound();
        }
    }
}
