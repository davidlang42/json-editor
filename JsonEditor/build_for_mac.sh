#!/bin/bash
dotnet publish -f net7.0-maccatalyst -c Release -p:MtouchLink=SdkOnly -p:CreatePackage=true -p:EnableCodeSigning=true -p:EnablePackageSigning=true -p:CodesignKey="Developer ID Application: David Lang (45435R99CA)" -p:CodesignProvision="JsonEditor" -p:CodesignEntitlements="Platforms\MacCatalyst\Entitlements.plist" -p:PackageSigningKey="Developer ID Installer: David Lang (45435R99CA)" -p:UseHardenedRuntime=true
PKG_FILE="`ls -1 ./bin/Release/net7.0-maccatalyst/publish/JsonEditor*.pkg | tail -n 1`"
echo Press ENTER to submit to Apple: $PKG_FILE
read
xcrun notarytool submit $PKG_FILE --wait --apple-id davidlang42@gmail.com --team-id 45435R99CA # this will prompt for an AppleID app specific password
xcrun stapler staple $PKG_FILE
xcrun stapler validate $PKG_FILE