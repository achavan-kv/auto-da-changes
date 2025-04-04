﻿













//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Cosacs Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Blue.Transactions;

namespace Blue.Cosacs.File
{
    
        public partial class ContextBase : DbContextBase
        {
			protected ContextBase(string connectionString = "Default") : base(connectionString) 
			{
				Database.SetInitializer<Context>(null);
			}
		 
            
                public virtual DbSet<FileDescription> FileDescription { get; set; }
            
            
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                
                        var t0 = modelBuilder.Entity< FileDescription >();
                        t0.ToTable("FileDescription", "File");
                        
                        t0.HasKey(t => t.Id);
                        
                        t0.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.Identity)
                        
                        
                        
                        
                        ;
                        
                        t0.Property(t => t.FileId)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
                        
                        
                        
                        
                        ;
                        
                        t0.Property(t => t.FileName)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
                        .IsUnicode(false)
                        .HasMaxLength(200)
                        
                        
                        ;
                        
                        t0.Property(t => t.FileSize)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
                        
                        
                        
                        
                        ;
                        
                        t0.Property(t => t.FileContent)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
                        
                        
                        
                        
                        ;
                        
                        t0.Property(t => t.FileExtension)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
                        .IsUnicode(false)
                        .HasMaxLength(10)
                        
                        
                        ;
                        
                        t0.Property(t => t.FileContentType)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
                        .IsUnicode(false)
                        .HasMaxLength(100)
                        
                        
                        ;
                        
                        t0.Property(t => t.CreatedOn)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
                        
                        
                        
                        
                        ;
                        
                        t0.Property(t => t.CreatedBy)
                        .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
                        
                        
                        
                        
                        ;
                        
            }
        }

        
        [Serializable]
	[DataContract]
    public partial class FileDescription
    {
                [DataMember] public int Id { get; set; }
                [DataMember] public Guid FileId { get; set; }
                [DataMember] public string FileName { get; set; }
                [DataMember] public long FileSize { get; set; }
                [DataMember] public byte[] FileContent { get; set; }
                [DataMember] public string FileExtension { get; set; }
                [DataMember] public string FileContentType { get; set; }
                [DataMember] public DateTime? CreatedOn { get; set; }
                [DataMember] public int? CreatedBy { get; set; }
            }

    

    }
