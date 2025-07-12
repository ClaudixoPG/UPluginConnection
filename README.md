
This repository contains the **integration of a native Android plugin into Unity** to process smartwatch inputs and use them as real-time game controls. It focuses on embedding the plugin, interpreting incoming data, mapping it to Unity's Input System, and visualizing the results within the game interface.

The implementation includes:

- **Plugin Embedding:** Integrating the compiled native Android plugin into Unity as a library.
- **Data Interpretation:** Processing raw input data into structured commands usable by Unity.
- **Input System Integration:** Mapping interpreted data to Unity’s Input System for real-time gameplay interaction.
- **Visualization:** Displaying smartwatch input data visually in the game UI for feedback and validation.

## 🚀 Main Objectives

- Embed and configure the Android plugin for Unity compatibility.
- Implement a data interpreter to translate plugin outputs into game inputs.
- Replicate smartwatch interactions within Unity in real time.
- Visualize and validate inputs through the Unity interface.

## 📂 Structure

- `/Assets/Plugins/Android`: Native plugin files integrated into Unity.
- `/Assets/Scripts`: Data interpreter and Input System integration scripts.

📑 Use Cases

### 🔌 Use Case: Plugin embedding

| **Field**         | **Description**                                                                                                                                                                                                            |
| ----------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Use Case**      | Embed native Android plugin into Unity.                                                                                                                                                                                    |
| **Actor**         | System.                                                                                                                                                                                                                    |
| **Purpose**       | Integrate the native Android plugin into the Unity project.                                                                                                                                                                |
| **Summary**       | The native Android plugin is created after verifying communication in Stage 1. The plugin is then embedded within Unity for internal use. The use case ends when Unity successfully accesses and uses the embedded plugin. |
| **Preconditions** | (1) Plugin compiled and encapsulated correctly. (2) Unity project configured for plugin integration.                                                                                                                       |
 
### 🧠 Use Case: Data interpretation in Unity                                                                                                                                                                                          

| **Field**         | **Description**                                                                                                                                                                                                                         |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Use Case**      | Interpret information received from the plugin in Unity.                                                                                                                                                                                |
| **Actor**         | System.                                                                                                                                                                                                                                 |
| **Purpose**       | Process the received data to generate valid input events in the game.                                                                                                                                                                   |
| **Summary**       | Unity accesses the embedded plugin, extracts the data, and the Interpreter module processes it to translate into input events recognizable by the Input System in real time. The use case ends when data is interpreted without errors. |
| **Preconditions** | (1) Embedded plugin available. (2) Interpreter module configured.                                                                                                                                                                       |

### 🎯 Use Case: Input System integration

| **Field**         | **Description**                                                                                                                                                                                                       |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Use Case**      | Integrate interpreted data into Unity Input System.                                                                                                                                                                   |
| **Actor**         | System.                                                                                                                                                                                                               |
| **Purpose**       | Replicate smartwatch input data as real-time inputs in Unity.                                                                                                                                                         |
| **Summary**       | The interpreted data is integrated into Unity’s Input System to replicate smartwatch interactions in real time within the game environment. The use case ends when inputs are correctly recognized by the game logic. |
| **Preconditions** | (1) Data interpreted. (2) Input System ready to receive events.                                                                                                                                                       |

### 🖥️ Use Case: Visualization in Unity
                                                                                                                                                                                          
| **Field**         | **Description**                                                                                                                                                                                     |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Use Case**      | Visualize interpreted data in Unity.                                                                                                                                                                |
| **Actor**         | System.                                                                                                                                                                                             |
| **Purpose**       | Display smartwatch input data in real-time within the Unity game interface.                                                                                                                         |
| **Summary**       | After data interpretation and Input System integration, Unity updates the game UI or visuals to reflect smartwatch inputs in real time. The use case ends when visualization is rendered correctly. |
| **Preconditions** | (1) Data interpreted and integrated. (2) UI components implemented.                                                                                                                                 |
                                                                                                                                                                                          |
## Notes

This repository is part of a Research Article titled "A Communication Module for Smartphone–Smartwatch Integration in Pervasive Games Using Unity 3D and Native Android", for more details about each stage and the performance analyses performed, download the associated paper (ref to the paper's doi)

## Authors

* **Claudio Rubio Naranjo** - [ClaudixoPG](https://github.com/ClaudixoPG); crubio17@alumnos.utalca.cl; claudiorubio23@gmail.com
* **Felipe Besoain** - [Fbesoain](https://github.com/fbesoain); fbesoain@utalca.cl

## Acknowledgments

* Research funded by Agencia Nacional de Investigación y Desarrollo, ANID-Subdirección del Capital Humano/Doctorado Nacional/2023-21232404 and FONDECYT Iniciación grant 11220438.

- Universidad de Talca
