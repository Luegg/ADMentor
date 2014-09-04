﻿using EAAddInFramework.MDGBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdAddIn.ADTechnology
{
    public static class TaggedValues
    {
        public class OrganisationalReachValue : Enumeration
        {
            public static readonly OrganisationalReachValue Global = new OrganisationalReachValue("Global");
            public static readonly OrganisationalReachValue Organisation = new OrganisationalReachValue("Organisation");
            public static readonly OrganisationalReachValue Program = new OrganisationalReachValue("Program");
            public static readonly OrganisationalReachValue Project = new OrganisationalReachValue("Project");
            public static readonly OrganisationalReachValue Subproject = new OrganisationalReachValue("Subproject");
            public static readonly OrganisationalReachValue BusinessUnit = new OrganisationalReachValue("Business Unit");
            public static readonly OrganisationalReachValue Individual = new OrganisationalReachValue("Individual");

            public static readonly IEnumerable<OrganisationalReachValue> All = new[] {
                Global, Organisation, Program, Project, Subproject, BusinessUnit, Individual
            };

            private OrganisationalReachValue(String name) : base(name) { }
        }

        public static readonly TaggedValue IntellectualPropertyRights = new TaggedValue(name: "Intellectual Property Rights", type: TaggedValueTypes.String);

        public static readonly TaggedValue OrganisationalReach = new TaggedValue(name: "Organisational Reach", type: TaggedValueTypes.Enum(OrganisationalReachValue.All));
    }
}
