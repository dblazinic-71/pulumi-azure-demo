using Pulumi;
using Azure = Pulumi.Azure;

class DemoStack : Stack
{
    public DemoStack()
    {
        var config = new Config();
        var prefix = config.Get("prefix") ?? "tfvmex";
        var mainResourceGroup = new Azure.Core.ResourceGroup("pulumiMainResourceGroup", new Azure.Core.ResourceGroupArgs
        {
            Location = "Germany West Central",
        });
        var mainVirtualNetwork = new Azure.Network.VirtualNetwork("pulumiMainVirtualNetwork", new Azure.Network.VirtualNetworkArgs
        {
            AddressSpaces =
            {
                "10.0.0.0/16",
            },
            Location = mainResourceGroup.Location,
            ResourceGroupName = mainResourceGroup.Name,
        });
        var @internal = new Azure.Network.Subnet("internal", new Azure.Network.SubnetArgs
        {
            ResourceGroupName = mainResourceGroup.Name,
            VirtualNetworkName = mainVirtualNetwork.Name,
            AddressPrefixes =
            {
                "10.0.2.0/24",
            },
        });
        var mainNetworkInterface = new Azure.Network.NetworkInterface("pulumiMainNetworkInterface", new Azure.Network.NetworkInterfaceArgs
        {
            Location = mainResourceGroup.Location,
            ResourceGroupName = mainResourceGroup.Name,
            IpConfigurations =
            {
                new Azure.Network.Inputs.NetworkInterfaceIpConfigurationArgs
                {
                    Name = "testconfiguration1",
                    SubnetId = @internal.Id,
                    PrivateIpAddressAllocation = "Dynamic",
                },
            },
        });
        var mainVirtualMachine = new Azure.Compute.VirtualMachine("pulumiMainVirtualMachine", new Azure.Compute.VirtualMachineArgs
        {
            Location = mainResourceGroup.Location,
            ResourceGroupName = mainResourceGroup.Name,
            NetworkInterfaceIds =
            {
                mainNetworkInterface.Id,
            },
            VmSize = "Standard_DS1_v2",
            StorageImageReference = new Azure.Compute.Inputs.VirtualMachineStorageImageReferenceArgs
            {
                Publisher = "Canonical",
                Offer = "UbuntuServer",
                Sku = "18.04-LTS",
                Version = "latest",
            },
            StorageOsDisk = new Azure.Compute.Inputs.VirtualMachineStorageOsDiskArgs
            {
                Name = "myosdisk1",
                Caching = "ReadWrite",
                CreateOption = "FromImage",
                ManagedDiskType = "Standard_LRS",
            },
            OsProfile = new Azure.Compute.Inputs.VirtualMachineOsProfileArgs
            {
                ComputerName = "hostname",
                AdminUsername = "testadmin",
                AdminPassword = "Password1234!",
            },
            OsProfileLinuxConfig = new Azure.Compute.Inputs.VirtualMachineOsProfileLinuxConfigArgs
            {
                DisablePasswordAuthentication = false,
            },
            Tags =
            {
                { "environment", "staging" },
            },
        });
    }

}