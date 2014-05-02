HiringManager
=============

<link rel="stylesheet" href="http://netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css" />

# Powershell Scripts

## Invoke-MigrateDB.ps1

<div class="well">
    Creates and/or migrates a database to the target version.
</div>

### Parameters

<ul>
    <li>
        <h4>databaseName</h4>
        <dl>
            <dt>Required</dt>
            <dd>false</dd>
            <dt>Default Value</dt>
            <dd>"HiringManager"</dd>
            <dt>Notes</dt>
            <dd>
                <div class="well">
                    The generated database name is comprised of 3 parts:
                    databaseName + environment + "Db"
                </div>
            </dd>
        </dl>
    </li>
    <li>
        <h4>buildType</h4>
        <dl>
            <dt>Required</dt>
            <dd>false</dd>
            <dt>Default Value</dt>
            <dd>"Debug"</dd>
            <dt>Notes</dt>
            <dd>
                <div class="well">
                    Helps the script find the compiled assembly to use for migrations.
                </div>
            </dd>
        </dl>
    </li>
    <li>
        <h4>targetMigration</h4>
        <dl>
            <dt>Required</dt>
            <dd>false</dd>
            <dt>Default Value</dt>
            <dd>""</dd>
            <dt>Notes</dt>
            <dd>
                <div class="well">
                    Passed through to Entity Framework Migrations
                </div>
            </dd>
        </dl>
    </li>
    <li>
        <h4>entityFrameworkVersion</h4>
        <dl>
            <dt>Required</dt>
            <dd>false</dd>
            <dt>Default Value</dt>
            <dd>"6.1.0"</dd>
            <dt>Notes</dt>
            <dd>
                <div class="well">
                    Helps the script find the Entity Framework Migations executable.
                </div>
            </dd>
        </dl>
    </li>
    <li>
        <h4>environment</h4>
        <dl>
            <dt>Required</dt>
            <dd>false</dd>
            <dt>Default Value</dt>
            <dd>""</dd>
            <dt>Notes</dt>
            <dd>
                <div class="well">
                    Helpful if you wish to have multiple copies of the database.
                    HiringManager integration tests are written assuming that there is a database called "HiringManagerIntegrationDb."
                    This strategy lets you use one copy of the database for development, and a separate copy of the database for integration tests.
                </div>
            </dd>
        </dl>
    </li>
</ul>

## Invoke-AddMigration.ps1

<div class="well">
    Creates a migration in the migrations assembly with the specified name.
</div>

### Parameters

<ul>
    <li>
        <h4>Name</h4>
        <dl>
            <dt>Required</dt>
            <dd>true</dd>
            <dt>Default Value</dt>
            <dd>""</dd>
            <dt>Notes</dt>
            <dd>
                <div class="well">
                    The generated database name is comprised of 3 parts:
                    databaseName + environment + "Db"
                </div>
            </dd>
        </dl>
    </li>
</ul>

## To add a migration and migrate your database

1. Make the necessary changes to the Entity Model.
2. Compile.
3. Execute ./Invoke-AddMigration.ps1 $Name
4. Compile.
5. Execute ./Invoke-MigrateDb.ps1
