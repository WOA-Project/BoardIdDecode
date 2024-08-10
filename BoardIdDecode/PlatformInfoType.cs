namespace BoardIdDecode
{
    enum PlatformInfoType
    {
        UNKNOWN = 0x00,     /**< Unknown target device. */
        CDP = 0x01,         /**< CDP device. */
        FFA = 0x02,         /**< Form-fit accurate device. */
        FLUID = 0x03,       /**< Forward looking user interface
                                           demonstration device. */
        OEM = 0x05,         /**< Original equipment manufacturer
                                                 device. */
        QT = 0x06,          /**< Qualcomm tablet device. */
        MTP = 0x08,         /**< MTP device. */
        LIQUID = 0x09,      /**< LiQUID device. */
        DRAGONBOARD = 0x0A, /**< DragonBoard@tm device. */
        QRD = 0x0B,         /**< QRD device. */
        EVB = 0x0C,         /**< EVB device. */
        HRD = 0x0D,         /**< HRD device. */
        DTV = 0x0E,  /**< DTV device. */
        RUMI = 0x0F, /**< Target is on Rumi (ASIC emulation). */
        VIRTIO = 0x10,  /**< Target is on Virtio
                                            (system-level simulation). */
        GOBI = 0x11, /**< Gobi@tm device. */
        CBH = 0x12,  /**< CBH device. */
        BTS = 0x13,  /**< BTS device. */
        XPM = 0x14,  /**< XPM device. */
        RCM = 0x15,  /**< RCM device. */
        DMA = 0x16,  /**< DMA device. */
        STP = 0x17,  /**< STP device. */
        SBC = 0x18,  /**< SBC device. */
        ADP = 0x19,  /**< ADP device. */
        CHI = 0x1A,  /**< CHI device. */
        SDP = 0x1B,  /**< SDP device. */
        RRP = 0x1C,  /**< RRP device. */
        CLS = 0x1D,  /**< CLS device. */
        TTP = 0x1E,  /**< TTP device. */
        HDK = 0x1F,  /**< HDK device. */
        IOT = 0x20,  /**< IOT device. */
        ATP = 0x21,  /**< ATP device. */
        IDP = 0x22,  /**< IDP device. */
        AEDK = 0x23, /**< AEDK device. */
        WDP = 0x24,  /**< WDP device. */
        QAM = 0x25,  /**< QAM device. */
        QXR = 0x26,  /**< QXR device. */
        X100 = 0x27, /**< Target is A PCIe card   */
        CRD = 0x28,   /**< CRD device   */
        QQVP = 0x29,   /**< Qualcomm QEMU Virtual Platform  */
    };
}