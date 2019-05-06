﻿using FreeSql.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FreeSql.PostgreSQL.Curd {

	class PostgreSQLDelete<T1> : Internal.CommonProvider.DeleteProvider<T1> where T1 : class {
		public PostgreSQLDelete(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere)
			: base(orm, commonUtils, commonExpression, dywhere) {
		}

		public override List<T1> ExecuteDeleted() {
			var sql = this.ToSql();
			if (string.IsNullOrEmpty(sql)) return new List<T1>();

			var sb = new StringBuilder();
			sb.Append(sql).Append(" RETURNING ");

			var colidx = 0;
			foreach (var col in _table.Columns.Values) {
				if (colidx > 0) sb.Append(", ");
				sb.Append(_commonUtils.QuoteReadColumn(col.Attribute.MapType, _commonUtils.QuoteSqlName(col.Attribute.Name))).Append(" as ").Append(_commonUtils.QuoteSqlName(col.CsName));
				++colidx;
			}
			sql = sb.ToString();
			var dbParms = _params.ToArray();
			var before = new Aop.CurdBeforeEventArgs(_table.Type, Aop.CurdType.Delete, sql, dbParms);
			_orm.Aop.CurdBefore?.Invoke(this, before);
			var ret = new List<T1>();
			Exception exception = null;
			try {
				ret = _orm.Ado.Query<T1>(_connection, _transaction, CommandType.Text, sql, dbParms);
			} catch (Exception ex) {
				exception = ex;
				throw ex;
			} finally {
				var after = new Aop.CurdAfterEventArgs(before, exception, ret);
				_orm.Aop.CurdAfter?.Invoke(this, after);
			}
			this.ClearData();
			return ret;
		}
		async public override Task<List<T1>> ExecuteDeletedAsync() {
			var sql = this.ToSql();
			if (string.IsNullOrEmpty(sql)) return new List<T1>();

			var sb = new StringBuilder();
			sb.Append(sql).Append(" RETURNING ");

			var colidx = 0;
			foreach (var col in _table.Columns.Values) {
				if (colidx > 0) sb.Append(", ");
				sb.Append(_commonUtils.QuoteReadColumn(col.Attribute.MapType, _commonUtils.QuoteSqlName(col.Attribute.Name))).Append(" as ").Append(_commonUtils.QuoteSqlName(col.CsName));
				++colidx;
			}
			sql = sb.ToString();
			var dbParms = _params.ToArray();
			var before = new Aop.CurdBeforeEventArgs(_table.Type, Aop.CurdType.Delete, sql, dbParms);
			_orm.Aop.CurdBefore?.Invoke(this, before);
			var ret = new List<T1>();
			Exception exception = null;
			try {
				ret = await _orm.Ado.QueryAsync<T1>(_connection, _transaction, CommandType.Text, sql, dbParms);
			} catch (Exception ex) {
				exception = ex;
				throw ex;
			} finally {
				var after = new Aop.CurdAfterEventArgs(before, exception, ret);
				_orm.Aop.CurdAfter?.Invoke(this, after);
			}
			this.ClearData();
			return ret;
		}
	}
}
