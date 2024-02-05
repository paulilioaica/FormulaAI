# Formula AI

## WIP

## Installation

```bash
git clone https://github.com/paulilioaica/FormulaAI
conda create -n mlagents python=3.10.12 && conda activate mlagents
cd FormulaAI
git clone --branch release_21 https://github.com/Unity-Technologies/ml-agents.git
pip install -r requirements.txt
python -m pip install ./ml-agents-envs --no-deps numpy
python -m pip install ./ml-agents --no-deps numpy
``` 

In order to start it, run the `TrainScene` in Unity version `2021.3.34f1`, then  =>
```bash
conda activate mlagents
mlagents-learn CarPPO.yaml --run-id="test"
```

and press play in Unity to start the training.




# Credits

* Initial simulation by [monidp9](https://github.com/monidp9)
* Graphics by [Kenney](https://www.kenney.nl/assets/racing-kit)
