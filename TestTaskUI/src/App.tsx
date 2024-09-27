import React, { useState } from "react";
import axios from "axios";
import "./App.css"; // Ensure to create this file for custom styles

const App: React.FC = () => {
  const [file, setFile] = useState<File | null>(null);
  const [token, setToken] = useState<string>("");
  const [headers, setHeaders] = useState<string[]>([]);
  const [selectedValues, setSelectedValues] = useState<{
    [key: string]: string;
  }>({});
  const [validationData, setValidationData] = useState<any>(null);
  const [importedData, setImportedData] = useState<any>(null); // State for the imported JSON data

  // Static fields and their labels
  const staticFields = [
    "Pickup store #",
    "Pickup store Name",
    "Pickup lat",
    "Pickup lon",
    "Pickup formatted Address",
    "Pickup Contact Name First Name",
    "Pickup Contact Name Last Name",
    "Pickup Contact Email",
    "Pickup Contact Mobile Number",
    "Pickup Enable SMS Notification",
    "Pickup Time",
    "Pickup tolerance (min)",
    "Pickup Service Time",
    "Delivery store #",
    "Delivery store Name",
    "Delivery lat (req if adding new customer)",
    "Delivery long (req if adding new customer)",
    "Delivery formatted Address",
    "Delivery Contact First Name",
    "Delivery Contact Last Name",
    "Delivery Contact Email",
    "Delivery Contact Mobile Number (need 0 at the front)",
    "Delivery Enable SMS Notification (No=0/Yes=1)",
    "Delivery Time",
    "Delivery Tolerance (Min past Delivery Time)",
    "Delivery Service Time (min)",
    "Order Details",
    "Assigned Driver",
    "Customer reference",
    "Payer",
    "Vehicle",
    "Weight",
    "Price",
  ];

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files) {
      setFile(event.target.files[0]);
    }
  };

  const handleFileUpload = async () => {
    if (!file) {
      alert("Please select a file.");
      return;
    }

    const formData = new FormData();
    formData.append("file", file);

    try {
      const response = await axios.post(
        "http://localhost:5000/api/fileprocessing/upload",
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );

      setToken(response.data.token);
      setHeaders(response.data.headers || []);
      alert("File uploaded successfully!");
    } catch (error) {
      console.error("File upload failed", error);
    }
  };

  const handleValidateFile = async () => {
    if (!token) {
      alert("Please upload a file first.");
      return;
    }

    try {
      const response = await axios.get(
        `http://localhost:5000/api/fileprocessing/validate-file/${token}`
      );
      setValidationData(response.data);
    } catch (error) {
      console.error("File validation failed", error);
    }
  };

  // Handle dropdown change
  const handleDropdownChange = (label: string, value: string) => {
    setSelectedValues((prev) => ({
      ...prev,
      [label]: value,
    }));
  };

  // Handle import button click
  const handleImport = async () => {
    if (!token) {
      alert("Please upload a file first.");
      return;
    }

    try {
      // Get the selected columns from the dropdowns
      const selectedColumns = Object.keys(selectedValues).map(
        (key) => selectedValues[key]
      );

      const response = await axios.post(
        `http://localhost:5000/api/fileprocessing/import/${token}`,
        selectedColumns
      );
      setImportedData(response.data); // Set the imported JSON data to state
    } catch (error) {
      console.error("Import failed", error);
    }
  };

  return (
    <div className="App">
      <h1>Excel File Upload</h1>

      <input type="file" onChange={handleFileChange} />
      <button onClick={handleFileUpload}>Upload File</button>

      <div className="container">
        {/* Left Column for Static Fields and Dropdowns */}
        <div className="left-column">
          {staticFields.map((field, index) => (
            <div key={index} className="dropdown-row">
              <span className="field-label">{field}:</span>
              <select
                onChange={(e) => handleDropdownChange(field, e.target.value)}
                disabled={headers.length === 0} // Disable dropdown if no headers
              >
                <option value="">Select a column</option>
                {headers.map((header, headerIndex) => (
                  <option key={headerIndex} value={header}>
                    {header}
                  </option>
                ))}
              </select>
            </div>
          ))}

          {/* Validate Button at the Bottom of Left Column */}
          <button
            onClick={handleValidateFile}
            disabled={!token}
            className="validate-button"
          >
            Validate File
          </button>

          {/* Import Button below Validate */}
          <button
            onClick={handleImport}
            disabled={!token || Object.keys(selectedValues).length === 0} // Disable if no token or selected columns
            className="validate-button" // Use the same class as the Validate button
          >
            Import
          </button>
        </div>

        {/* Right Column for Preview Data */}
        <div className="right-column">
          {importedData ? (
            <div>
              <h3>Imported Data</h3>
              <pre>{JSON.stringify(importedData.data, null, 2)}</pre>
            </div>
          ) : (
            validationData && (
              <div>
                <h3>File Validation Results</h3>
                <p>Total Rows: {validationData.totalRows}</p>
                <h4>First Row Preview:</h4>
                <div className="preview-container">
                  {staticFields.map((field, index) => (
                    <div key={index} className="preview-row">
                      <span className="header-label">{field}:</span>
                      <span className="header-value">
                        {validationData.previewRow
                          ? validationData.previewRow[field]
                          : "No data"}
                      </span>
                    </div>
                  ))}
                </div>
              </div>
            )
          )}
        </div>
      </div>
    </div>
  );
};

export default App;
