using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Autofac;
using ServiceStack;
using ServiceStack.OrmLite.SqlServer;


namespace oze.data
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            ServiceStackHelper.Help();
            LicenseUtils.ActivatedLicenseFeatures();
            builder.Register(c => new OzeConnectionFactory(ConfigurationManager.ConnectionStrings["V21ConnectionString"].ConnectionString, SqlServerOrmLiteDialectProvider.Instance)).As<IOzeConnectionFactory>();
        }
    }
}